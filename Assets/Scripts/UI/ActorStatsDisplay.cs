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
        damageSeq.Append(ForceState(Vector3.one * focusedSize, 0f, 0.025f));
        damageSeq.Append(ForceState(Vector3.one * unfocusedSize, 0.5f, 0.025f));
        damageSeq.Play();
    }
    
    private void Healed()
    {
        healSeq?.Kill();
        healSeq = DOTween.Sequence();
        // TODO: IMPLEMENT FOR HEALING DICES
    }

    private Sequence ForceState(Vector3 targetScale, float fadeValue, float duration)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(targetScale, focusTime));
        sequence.Join(focusMask.DOFade(fadeValue, duration));
        sequence.SetEase(Ease.OutBack);
        return sequence;
    }

    private void UpdateState(OnTurnStart evt)
    {
        bool isFocus = evt.actor.Order == ownerOrder;
        Vector3 targetScale = (isFocus ? focusedSize : unfocusedSize) * Vector3.one;
        float fadeValue = isFocus ? 0f : 0.5f;
        
        focusSeq?.Kill();
        focusSeq = ForceState(targetScale, fadeValue, focusTime);
        focusSeq.Play();
    }

    private void Setup(OnCreateActor evt)
    {
        if (evt.actor.Order != ownerOrder) return;

        cachedHealth = evt.actor.Health;
        healthText.SetText(cachedHealth.ToString());
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnActorHealthChange>(UpdateDisplay);
        EventsManager.AddSubscriber<OnTurnStart>(UpdateState);
        EventsManager.AddSubscriber<OnCreateActor>(Setup);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnActorHealthChange>(UpdateDisplay);
        EventsManager.RemoveSubscriber<OnTurnStart>(UpdateState);
        EventsManager.RemoveSubscriber<OnCreateActor>(Setup);
    }
}
