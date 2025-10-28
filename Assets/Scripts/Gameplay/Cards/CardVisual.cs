using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image artHolder;
    [SerializeField] private TMP_Text nickname, description;

    private CardLayout currentLayout;
    private CardRuntime info;
    private Tween moveTween;
    private bool isGoingTo, isOnFocus, isHovering;
    private const float kGotoDuration = 0.6f;

    public bool IsOnFocus => isOnFocus;
    public bool IsHovering => isHovering;

    public void Setup(CardRuntime info, CardLayout layout)
    {
        if (info == null) return;

        this.info = info;
        info.OnMove += GoTo;

        currentLayout = layout;

        artHolder.sprite = info.Artwork;
        nickname.SetText(info.Nickname);
        description.SetText(info.Description);
    }

    private void OnDestroy()
    {
        info.OnMove -= GoTo;
    }

    private void OnReachDestination()
    {
        currentLayout.AddCard(this);
        isGoingTo = false;
    }

    private void GoTo(Vector3 destinationWorld, CardLayout newLayout)
    {
        isGoingTo = true;

        currentLayout.RemoveCard(this);
        currentLayout = newLayout;

        moveTween?.Kill();
        moveTween = transform.DOMove(destinationWorld, kGotoDuration).SetEase(Ease.OutQuad);
        moveTween.OnComplete(OnReachDestination);
    }

    // Clicked
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isGoingTo) return;
        isOnFocus = !isOnFocus;
        currentLayout.UpdateLayout();
    }

    // Release Click
    public void OnPointerDown(PointerEventData eventData)
    {
        if (isGoingTo) return;

    }

    // Hover Start
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isGoingTo) return;
        isHovering = true;
        currentLayout.UpdateLayout();
    }

    // Hover Stop
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isGoingTo) return;
        isHovering = false;
        currentLayout.UpdateLayout();
    }
}
