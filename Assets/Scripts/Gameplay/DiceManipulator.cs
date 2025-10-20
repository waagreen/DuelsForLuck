using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceManipulator : MonoBehaviour
{
    [SerializeField] private List<Die> dice;

    [Header("Movement Settings")]
    [SerializeField][Range(0f, 50f)] private float followForce = 20f, maxSpeed = 30f, maxRollTorque = 10f;
    [SerializeField][Range(0f, 90f)] private float minTiltAngle = 0f, maxTiltAngle = 20f;
    [SerializeField][Min(0f)] private float cameraViewOffset = 10f;
    [SerializeField][Range(1f, 5f)] private float diceSpacing = 1f;

    [Header("Shake Settings")]
    [SerializeField][Range(0f, 10f)] private float shakeIntensity = 2f;
    [SerializeField][Range(0f, 20f)] private float shakeSpeed = 8f;

    private readonly List<Vector3> spreadValues = new();
    private Actions inputActions;
    private InputAction pointerAction;
    private Camera mainCam;
    private bool isDragging = false;
    private float shakeTimer;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnTurnStart>(TryBotPlay);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnTurnStart>(TryBotPlay);
    }

    private void Start()
    {
        mainCam = Camera.main;
        pointerAction = inputActions.FindAction("Pointer");
    }

    protected virtual void Drop()
    {
        isDragging = false;
        foreach (Die die in dice)
        {
            die.Throw(maxRollTorque, maxSpeed);
        }
    }

    protected virtual void Grab()
    {
        isDragging = true;
        shakeTimer = 0f;
        spreadValues.Clear();
        
        foreach (Die die in dice)
        {
            spreadValues.Add(Random.insideUnitSphere * diceSpacing);
            die.GoToHand();
        }
    }

    private void OnEnable()
    {   
        foreach (Die die in dice)
        {
            die.OnDrop += Drop;
            die.OnGrab += Grab;
        }

        inputActions = new();
        inputActions.Enable();
    }

    private void OnDisable()
    {
        foreach (Die die in dice)
        {
            die.OnDrop -= Drop;
            die.OnGrab -= Grab;
        }

        inputActions.Disable();
    }

    private void TryBotPlay(OnTurnStart evt)
    {
        if (!evt.currentActor.IsBot) return;

        Drop();
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 pointerPosition = pointerAction.ReadValue<Vector2>();
        Ray ray = mainCam.ScreenPointToRay(pointerPosition);

        Plane tablePlane = new(Vector3.up, new Vector3(0f, cameraViewOffset, 0f));

        if (tablePlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        // fallback
        return dice[0].Rb.position;
    }

    private void HandleTilt(Vector3 desiredVelocity, Rigidbody rb)
    {
        if (desiredVelocity.sqrMagnitude > 0.01f)
        {
            // Lerp between angles based on the current velocity
            float tiltAngle = Mathf.Lerp(minTiltAngle, maxTiltAngle, desiredVelocity.magnitude / maxSpeed);

            // Define a target rotation and interpolate with the object's current rotation
            Quaternion targetRot = Quaternion.LookRotation(desiredVelocity.normalized) * Quaternion.Euler(0f, tiltAngle, 0f);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, Time.deltaTime));
        }
        else
        {
            // Interpolate back to no rotation
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.identity, Time.deltaTime));
        }
    }
    
    private void HandleShake(Rigidbody rb)
    {
        shakeTimer += Time.deltaTime * shakeSpeed;

        Vector3 shakeTorque = rb.linearVelocity.magnitude * shakeIntensity * new Vector3
        (
            Mathf.Sin(shakeTimer * 1.7f),
            Mathf.Cos(shakeTimer * 1.3f),
            Mathf.Sin(shakeTimer * 2.1f)
        );

        rb.AddTorque(shakeTorque, ForceMode.Acceleration);
    }

    private void FixedUpdate()
    {
        if (!isDragging) return;

        Vector3 pointerWorld = GetPointerWorldPosition();
        int count = dice.Count;

        if (count == 0) return;

        float totalWidth = (count - 1) * diceSpacing;
        float startOffset = -totalWidth / 2f;

        for (int i = 0; i < dice.Count; i++)
        {
            Die die = dice[i];
            Rigidbody rb = die.Rb;

            // Each die is offset alternating between left and right
            float offsetX = startOffset + i * diceSpacing;
            Vector3 targetPosition = pointerWorld + offsetX * mainCam.transform.right;

            Vector3 direction = targetPosition - rb.position;
            Vector3 desiredVelocity = direction * followForce;

            // Clamp velocity if necessary
            if (desiredVelocity.magnitude > maxSpeed)
            {
                desiredVelocity = desiredVelocity.normalized * maxSpeed;
            }

            rb.linearVelocity = desiredVelocity;

            HandleShake(rb);
            HandleTilt(desiredVelocity, rb);
        }
    }
}
