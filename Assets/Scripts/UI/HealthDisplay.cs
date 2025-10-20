using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image focusMask;
    [SerializeField] private List<Image> winIndicators;

    [Header("Focus Settings")]
    [SerializeField] private Order ownerOrder;
    [SerializeField][Min(0f)] private float focusedSize = 1.2f, unfocusedSize = 0.8f, focusTime = 1f;

    private Sequence focusSeq, damageSeq, healSeq;
    private int cachedHealth;

    private void UpdateDisplay(OnPlayerHealthChange evt)
    {
        if (evt.turnOrder == null || evt.turnOrder.Value != ownerOrder) return;

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

    private void UpdateState(OnTurnChange evt)
    {
        focusSeq?.Kill();
        focusSeq = DOTween.Sequence();

        bool isFocus = evt.turnOrder == ownerOrder;
        Vector3 targetScale = (isFocus ? focusedSize : unfocusedSize) * Vector3.one;
        float fadeValue = isFocus ? 0f : 0.5f;

        focusSeq.Append(transform.DOScale(targetScale, focusTime));
        focusSeq.Join(focusMask.DOFade(fadeValue, focusTime));
        focusSeq.SetEase(Ease.OutBack);
        focusSeq.Play();
    }

    private void Setup(OnCreatePlayer evt)
    {
        if (evt.newPlayer.Order != ownerOrder) return;

        cachedHealth = evt.newPlayer.Health;
        healthText.SetText(cachedHealth.ToString());
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnPlayerHealthChange>(UpdateDisplay);
        EventsManager.AddSubscriber<OnTurnChange>(UpdateState);
        EventsManager.AddSubscriber<OnCreatePlayer>(Setup);

    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnPlayerHealthChange>(UpdateDisplay);
        EventsManager.RemoveSubscriber<OnTurnChange>(UpdateState);
        EventsManager.RemoveSubscriber<OnCreatePlayer>(Setup);
    }
}
