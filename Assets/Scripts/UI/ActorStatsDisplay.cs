using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActorStatsDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText, armorText;
    [SerializeField] private Image healthFill, armorFill;
    [SerializeField] private Transform armorIcon;

    [Header("Focus Settings")]
    [SerializeField] private int ownerOder;

    private Sequence healthSeq, armorSeq;
    private int maxHealth, cachedHealth, cachedArmor;

    private Sequence UpdateHealth(int dealtaHealth)
    {
        int previousHealth = cachedHealth;
        cachedHealth = Mathf.Clamp(cachedHealth + dealtaHealth, 0, dealtaHealth);

        healthSeq?.Kill();
        healthSeq = DOTween.Sequence();
        
        float fillAmount = cachedHealth / maxHealth;
        healthSeq.Append(healthFill.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.OutQuad));
        healthSeq.OnComplete(() => healthText.SetText($"{cachedHealth}/{maxHealth}"));
        
        return healthSeq;
    }

    private Sequence UpdateArmor(int dealtaArmor)
    {
        int previousArmor = cachedArmor;
        cachedArmor += dealtaArmor;

        armorSeq?.Kill();
        armorSeq = DOTween.Sequence();

        float fillAmount = (float)cachedArmor / maxHealth;
        if (previousArmor <= 0) armorSeq.Append(armorIcon.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        armorSeq.Append(armorFill.DOFillAmount(fillAmount, 0.5f).SetEase(Ease.OutQuad));
        if (cachedArmor <= 0) armorSeq.Append(armorIcon.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        armorSeq.OnComplete(() => armorText.SetText($"{cachedArmor}"));
        
        return armorSeq;
    }

    private void Setup(OnCreateActor evt)
    {
        if (ownerOder != evt.actor.Order) return;

        maxHealth = evt.actor.MaxHealth;
        cachedHealth = evt.actor.Health;
        cachedArmor = evt.actor.GetStatus(CardEffectType.Armor);

        healthText.SetText($"{cachedHealth}/{maxHealth}");
        armorText.SetText(cachedArmor.ToString());

        healthFill.fillAmount = cachedHealth / maxHealth;
        armorFill.fillAmount = cachedArmor / maxHealth;

        armorIcon.localScale = cachedArmor > 0 ? Vector3.one : Vector3.zero;
    }
    
    public Sequence GetEffectSequence(CardEffectType type, int delta)
    {
        return type switch
        {
            CardEffectType.Damage => UpdateHealth(delta),
            CardEffectType.Armor => UpdateArmor(delta),
            CardEffectType.Heal => UpdateHealth(delta),
            _ => null,
        };
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateActor>(Setup);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateActor>(Setup);
    }
}
