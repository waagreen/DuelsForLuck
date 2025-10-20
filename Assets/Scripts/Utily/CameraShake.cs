using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    private Camera cam;
    private Tween shakeTween;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCameraShake>(Shake);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCameraShake>(Shake);
    }

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void Shake(OnCameraShake evt)
    {
        shakeTween?.Kill();
        shakeTween = cam.DOShakePosition(evt.duration, evt.strength, evt.vibrato, evt.randomness);
    }
}
