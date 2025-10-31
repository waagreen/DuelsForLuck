using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Die : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private MeshRenderer mesh;
    [SerializeField][Min(0f)] private float removeDuration = 0.1f, selectDuration = 0.15f;
    [SerializeField][Min(0f)] private float selectScale = 1.1f, baseScale = 0.75f;
    [SerializeField][Min(0f)] private float jumpPower = 1f, jumpScale = 0.25f;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private List<AudioClip> collisionSounds;
    [SerializeField][Min(0f)] private float minImpactVolume = 0.15f, impactCooldown = 0.1f;

    private Sequence colorSeq, selectSeq, removeSeq;
    private Color cachedColor;
    
    private Collider col;
    private Rigidbody rb;
    private PhysicsMaterial cachedMaterial;
    
    private bool wasThrown, hasValue, isSelected, isOutOfPlay;
    private int value;
    private float lastImpact;

    public bool CanInteract() => (wasThrown == false) && hasValue;
    
    private readonly Vector3[] faceNormals = new Vector3[]
    {
        Vector3.down,
        Vector3.left,
        Vector3.back,
        Vector3.forward,
        Vector3.right,
        Vector3.up
    };

    public Rigidbody Rb => rb;
    public event System.Action OnGrab;
    public event System.Action OnDrop;

    private void InitializeForTurn(OnTurnStart evt)
    {
        hasValue = false;
        isOutOfPlay = false;
        isSelected = false;
        wasThrown = false;

        cachedColor = (evt.actor.Order == 0) ? Actor.PColor : Actor.BotColor;
        transform.localScale = Vector3.one * baseScale;
        ColorFaces(cachedColor);
    }

    private Sequence SelectSequence()
    {
        Vector3 targetScale = Vector3.one * (isSelected ? selectScale : baseScale);
        Vector3 targetJump = transform.position + (Vector3.up * jumpScale);
        Color targetColor = isSelected ? Color.white : cachedColor;

        selectSeq?.Kill();
        selectSeq = DOTween.Sequence();
        
        selectSeq.Append(transform.DOScale(targetScale, selectDuration).SetEase(Ease.OutBack));
        if (isSelected) selectSeq.Join(transform.DOLocalJump(targetJump, jumpPower, numJumps: 1, selectDuration).SetEase(Ease.OutCubic));
        selectSeq.Join(ColorFaces(targetColor));
        
        return selectSeq;
    }

    private void ToggleSelection()
    {
        if (isOutOfPlay) return;

        isSelected = !isSelected;
        SelectSequence();
    }
    
    private void RemoveFromPlay(OnPlayCard evt)
    {
        if (!isSelected) return;

        float targetJump = (transform.position + (Vector3.up * jumpScale)).y;
        
        removeSeq?.Kill();
        removeSeq = DOTween.Sequence();

        removeSeq.Append(transform.DOLocalMoveY(targetJump, removeDuration).SetEase(Ease.OutQuad));
        removeSeq.Append(transform.DOScale(Vector3.zero, removeDuration / 3f).SetEase(Ease.InCirc));
        removeSeq.OnComplete(() => isOutOfPlay = true);
        removeSeq.Play();
    }

    // Player clicked on another die, and this one was already selected
    private void Disselect(OnSelectDie evt)
    {
        if (evt.id == gameObject.GetInstanceID()) return;
        if (isSelected == false) return;
        if (isOutOfPlay) return;

        isSelected = false;
        SelectSequence();
    }

    public void OnMouseDown()
    {
        if (!CanInteract()) return;

        OnGrab?.Invoke();
        ToggleSelection();
            
        if (isSelected) EventsManager.Broadcast(new OnSelectDie { value = this.value, id = gameObject.GetInstanceID() });
        else EventsManager.Broadcast(new OnDisselectDie()); 
    }

    public void OnMouseUp()
    {
        if (!CanInteract()) return;
        
        OnDrop?.Invoke();
    }

    private Sequence ColorFaces(Color colr)
    {
        colorSeq?.Kill();
        colorSeq = DOTween.Sequence();

        colorSeq.Join(mesh.material.DOColor(colr, 0.2f));
        colorSeq.SetEase(Ease.OutCubic);

        return colorSeq;
    }

    public void Throw(float maxRollTorque)
    {
        wasThrown = true;
        rb.useGravity = true;

        Vector3 aditionalAxis = (Random.Range(0f, 1f) > 0.5f) ? transform.right : transform.up;
        Vector3 torque = maxRollTorque * (transform.forward + aditionalAxis);

        rb.AddTorque(torque, ForceMode.Impulse);
        rb.AddForce(rb.linearVelocity * 0.5f, ForceMode.Impulse);
    }

    private int GetTopFace()
    {
        int topFace = 0;
        float maxDot = -Mathf.Infinity;

        for (int i = 0; i < faceNormals.Length; i++)
        {
            Vector3 worldDirection = transform.TransformDirection(faceNormals[i]);
            float dot = Vector3.Dot(worldDirection, Vector3.up);

            if (dot > maxDot)
            {
                maxDot = dot;
                topFace = i + 1;
            }
        }

        if (topFace == 1) return 0;
        else if (topFace == 6) return 2;
        else return 1;
    }

    private void HandleCollisionSound(Collision collision)
    {
        if ((audioScr == null) || (collisionSounds == null) || (collisionSounds.Count < 1)) return;
        
        float now = Time.time;
        if (now - lastImpact < impactCooldown) return;

        float impactStrenght = collision.relativeVelocity.magnitude;
        float volume = Mathf.Clamp01(impactStrenght / 10f);

        if (volume > minImpactVolume)
        {
            audioScr.volume = volume;
            audioScr.pitch = Random.Range(0.85f, 1.1f);
            audioScr.clip = collisionSounds[Random.Range(0, collisionSounds.Count)];
            audioScr.Play();
            lastImpact = now;
        }
    }

    private void FixedUpdate()
    {
        if (wasThrown && (rb.linearVelocity == Vector3.zero))
        {
            value = GetTopFace();
            Debug.Log($"Topface {value}");
            wasThrown = false;
            hasValue = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionSound(collision);
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnTurnStart>(InitializeForTurn);
        EventsManager.AddSubscriber<OnSelectDie>(Disselect);
        EventsManager.AddSubscriber<OnPlayCard>(RemoveFromPlay);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnTurnStart>(InitializeForTurn);
        EventsManager.RemoveSubscriber<OnSelectDie>(Disselect);
        EventsManager.RemoveSubscriber<OnPlayCard>(RemoveFromPlay);
    }

    private void Start()
    {
        transform.localScale = Vector3.one * baseScale;
        
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
}