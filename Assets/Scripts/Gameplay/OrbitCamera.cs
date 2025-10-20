using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;


public class OrbitCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform pitchPivot;
    [SerializeField] private Transform cameraTransform;

    [Header("Orbit Setting")]
    [SerializeField][Min(0f)] private float radius = 5f;
    [SerializeField][Min(0f)] private float height = 2f;
    [SerializeField][Range(-270f, 270f)] private float initialPitch = 35f, initialYaw = 270f;

    private Tween orbitTween, pitchTween, shakeTween;
    private float currentYaw, currentPitch;
    private Camera attachedCamera;

    private void Start()
    {
        currentPitch = initialPitch;
        currentYaw = initialYaw;
        UpdatePositions();

        if (cameraTransform.TryGetComponent(out Camera cam))
        {
            attachedCamera = cam;
        }
        else
        {
            cameraTransform.AddComponent<Camera>();
        }
    }

    public Tween OrbitToAngle(float targetYaw, float duration)
    {
        orbitTween?.Kill();
        orbitTween = DOTween.To
        (
            () => currentYaw, // Getter
            x => // Setter
            {
                currentYaw = x;
                UpdatePositions();
            },
            targetYaw, // Endvalue
            duration
        );

        return orbitTween;
    }

    public Tween AdjustViewAngle(float targetPitch, float duration)
    {
        pitchTween?.Kill();
        pitchTween = DOTween.To
        (
            () => currentPitch, // Getter
            x => // Setter
            {
                currentPitch = x;
                UpdatePitch();
            },
            targetPitch, // Endvalue
            duration
        );

        return pitchTween;
    }

    private void UpdatePositions()
    {
        float rad = currentYaw * Mathf.Deg2Rad;

        Vector3 orbitPos = new
        (
            Mathf.Sin(rad) * radius,
            height,
            Mathf.Cos(rad) * radius
        );

        transform.position = target.position + orbitPos;
        transform.LookAt(target);

        UpdatePitch();
    }

    private void UpdatePitch()
    {
        pitchPivot.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    public Tween Shake(float duration, float strength = 3, int vibrato = 10, float randomness = 90)
    {
        shakeTween?.Kill();

        shakeTween = attachedCamera.DOShakePosition(duration, strength, vibrato, randomness);
        return shakeTween;
    }
}
