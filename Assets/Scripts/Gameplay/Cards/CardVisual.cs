using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image artHolder;
    [SerializeField] private TMP_Text nickname, description, cost;
    [SerializeField] private List<Graphic> visualElements;

    private CardLayout currentLayout;
    private CardRuntime info;
    private Sequence moveSeq, stateSeq;
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
        cost.SetText(info.Cost.ToString());
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

        moveSeq?.Kill();
        moveSeq = DOTween.Sequence();
        moveSeq.Append(transform.DOMove(currentLayout.transform.position, kGotoDuration));
        moveSeq.Join(transform.DORotateQuaternion(currentLayout.transform.rotation, kGotoDuration));
        moveSeq.SetEase(Ease.OutQuad);
        moveSeq.OnComplete(OnReachDestination);
    }

    public void UpdateActiveState(bool flag)
    {
        stateSeq?.Kill();
        stateSeq = DOTween.Sequence();
        Color color = flag ? Color.white : Color.grey;

        foreach (Graphic graphic in visualElements)
        {
            stateSeq.Join(graphic.DOColor(color, kGotoDuration));
        }
        stateSeq.SetEase(Ease.OutQuad);
        stateSeq.Play();
    }

    public void UpdateCostState(int selectedDieValue)
    {
        if (selectedDieValue < 0)
        {
            cost.color = Color.white;
            UpdateActiveState(false);
        }
        else
        {
            bool canUse = selectedDieValue >= info.Cost;
            cost.color = canUse ? Color.green : Color.red;
            UpdateActiveState(canUse);
        }
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
