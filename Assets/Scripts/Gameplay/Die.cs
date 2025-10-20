using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Die : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private List<SpriteRenderer> faces;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private List<AudioClip> collisionSounds;
    [SerializeField][Min(0f)] private float minImpactVolume = 0.15f, impactCooldown = 0.1f;

    private Sequence colorSequence;
    private Color cachedColor;
    
    private Collider col;
    private Rigidbody rb;
    private PhysicsMaterial cachedMaterial;
    
    private bool wasThrown = false;
    private int value;
    private float lastImpact;
    
    private readonly Vector3[] faceNormals = new Vector3[]
    {
        Vector3.up,
        Vector3.left,
        Vector3.back,
        Vector3.forward,
        Vector3.right,
        Vector3.down
    };

    public Rigidbody Rb => rb;
    public event System.Action OnGrab;
    public event System.Action OnDrop;

    private void UpdateTurnVisuals(OnTurnChange evt)
    {
        cachedColor = evt.turnOrder == Order.First ? Player.P1Color : Player.P2Color;    
        ColorFaces(cachedColor);
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnTurnChange>(UpdateTurnVisuals);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnTurnChange>(UpdateTurnVisuals);
    }

    private void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnMouseDown()
    {
        if (wasThrown) return;
        OnGrab?.Invoke();
    }
    public void OnMouseUp()
    {
        if (wasThrown) return;
        OnDrop?.Invoke();
    }

    private void ColorFaces(Color colr)
    {
        colorSequence?.Kill();
        colorSequence = DOTween.Sequence();
        colorSequence.AppendInterval(0.1f);

        foreach (SpriteRenderer rend in faces)
        {
            colorSequence.Join(rend.DOColor(colr, 0.2f));
        }

        colorSequence.SetEase(Ease.OutCubic);
        colorSequence.Play();
    }

    public void Throw(float maxRollTorque, float maxSpeed)
    {
        col.sharedMaterial = cachedMaterial;

        rb.useGravity = true;
        
        float rollIntensity = Mathf.Lerp(0, maxRollTorque, rb.linearVelocity.magnitude / maxSpeed);
        Vector3 randomTorque = Random.insideUnitSphere * rollIntensity;

        rb.AddTorque(randomTorque, ForceMode.Impulse);
        rb.AddForce(rb.linearVelocity * 0.5f, ForceMode.Impulse);

        wasThrown = true;
    }

    public void GoToHand()
    {
        wasThrown = false;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.rotation = Quaternion.identity;

        cachedMaterial = col.sharedMaterial;
        col.sharedMaterial = null;
    
        ColorFaces(cachedColor);
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

        return topFace;
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
            wasThrown = false;
            value = GetTopFace();
            
            EventsManager.Broadcast(new OnDieResult { result = value });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleCollisionSound(collision);
    }
}