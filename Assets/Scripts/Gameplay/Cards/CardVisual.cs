using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image artHolder;
    [SerializeField] private TMP_Text nickname, description;

    private CardLayout currentLayout;
    private CardRuntime info;
    private Tween moveTween;
    private const float kGotoDuration = 0.6f;

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

    private void GoTo(Vector3 destinationWorld, CardLayout newLayout)
    {
        currentLayout.RemoveCard(this);
        currentLayout = newLayout;
        
        moveTween?.Kill();
        moveTween = transform.DOMove(destinationWorld, kGotoDuration).SetEase(Ease.OutQuad);
        moveTween.OnComplete(() => newLayout.AddCard(this));
    }
    
}
