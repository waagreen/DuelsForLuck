using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActorStatsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image focusMask;
    [SerializeField] private List<Image> winIndicators;

    [Header("Focus Settings")]
    [SerializeField] private int ownerOrder;
    [SerializeField][Min(0f)] private float focusedSize = 1.2f, unfocusedSize = 0.8f, focusTime = 1f;

    private Sequence focusSeq, damageSeq, healSeq;
    private int cachedHealth;

    private void UpdateDisplay(OnActorHealthChange evt)
    {
        if ((evt.turnIndex != null) && (evt.turnIndex.Value != ownerOrder)) return;

        int previousHealth = cachedHealth;
        
        cachedHealth += evt.dealtaHealth;
        healthText.SetText(cachedHealth.ToString());

        if (previousHealth > cachedHealth) TakenDamage();
        else Healed();
    }

    private void TakenDamage()
    {
        damageSeq?.Kill();
        damageSeq = DOTween.Sequence();
        damageSeq.Append(transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, vibrato: 3, elasticity: 0.3f));
        damageSeq.Play();
    }
    
    private void Healed()
    {
        healSeq?.Kill();
        healSeq = DOTween.Sequence();
        // TODO: IMPLEMENT FOR HEALING DICES
    }

    private void UpdateState(OnImmediateTurnChange evt)
    {
        focusSeq?.Kill();
        focusSeq = DOTween.Sequence();

        bool isFocus = evt.turnIndex == ownerOrder;
        Vector3 targetScale = (isFocus ? focusedSize : unfocusedSize) * Vector3.one;
        float fadeValue = isFocus ? 0f : 0.5f;

        focusSeq.Append(transform.DOScale(targetScale, focusTime));
        focusSeq.Join(focusMask.DOFade(fadeValue, focusTime));
        focusSeq.SetEase(Ease.OutBack);
        focusSeq.Play();
    }

    private void Setup(OnCreateActor evt)
    {
        if (evt.newActor.Order != ownerOrder) return;

        cachedHealth = evt.newActor.Health;
        healthText.SetText(cachedHealth.ToString());
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnActorHealthChange>(UpdateDisplay);
        EventsManager.AddSubscriber<OnImmediateTurnChange>(UpdateState);
        EventsManager.AddSubscriber<OnCreateActor>(Setup);

    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnActorHealthChange>(UpdateDisplay);
        EventsManager.RemoveSubscriber<OnImmediateTurnChange>(UpdateState);
        EventsManager.RemoveSubscriber<OnCreateActor>(Setup);
    }
}
