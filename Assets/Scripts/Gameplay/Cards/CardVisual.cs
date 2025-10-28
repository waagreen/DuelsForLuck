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
    private Sequence moveTween;
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

    private void ResetAllFlags()
    {
        isGoingTo = isOnFocus = isHovering = false;
    }

    private void OnReachDestination()
    {
        ResetAllFlags();
        currentLayout.AddCard(this);
    }

    private void GoTo(CardLayout newLayout)
    {
        isGoingTo = true;

        currentLayout.RemoveCard(this);
        currentLayout = newLayout;

        moveTween?.Kill();
        moveTween = DOTween.Sequence();
        moveTween.Append(transform.DOMove(currentLayout.transform.position, kGotoDuration));
        moveTween.Join(transform.DORotateQuaternion(currentLayout.transform.rotation, kGotoDuration));
        moveTween.SetEase(Ease.OutQuad);
        moveTween.OnComplete(OnReachDestination);
    }

    // Clicked
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isGoingTo) return;
        isOnFocus = !isOnFocus;
        currentLayout.UpdateLayout();

        if (currentLayout.IsPickable) EventsManager.Broadcast(new OnPickDrawCard { pickedCard = info });
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
