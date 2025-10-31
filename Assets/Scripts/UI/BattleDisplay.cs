using DG.Tweening;
using UnityEngine;

public class BattleDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas display;
    [SerializeField] private RectTransform diceHolder;
    [SerializeField] private ActorModel model;
    [SerializeField][Range(0, 1)] private int damageFrom;


    private Sequence Damage()
    {
        Sequence effectSeq = DOTween.Sequence();

        effectSeq.Append(model.Hurt());
        effectSeq.Join(model.Colorize(Color.white));
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.Damage));
        
        return effectSeq;
    }

    private Sequence Armor()
    {
        Sequence effectSeq = DOTween.Sequence();
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.Armor));

        return effectSeq;
    }

    private Sequence Heal()
    {
        Sequence effectSeq = DOTween.Sequence();
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.Heal));

        return effectSeq;
    }

    private Sequence Poison()
    {
        Sequence effectSeq = DOTween.Sequence();

        effectSeq.Join(model.Colorize(Color.magenta));
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.Poison));

        return effectSeq;
    }

    private Sequence Reroll()
    {
        Sequence effectSeq = DOTween.Sequence();
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.Reroll));

        return effectSeq;
    }

    private Sequence StoreEnergy()
    {
        Sequence effectSeq = DOTween.Sequence();
        effectSeq.JoinCallback(() => model.PlaySound(CardEffectType.StoreEnergy));

        return effectSeq;
    }
    
    public Sequence GetEffectSequence(CardEffectType type)
    {
        return type switch
        {
            CardEffectType.Damage => Damage(),
            CardEffectType.Armor => Armor(),
            CardEffectType.Heal => Heal(),
            CardEffectType.Poison => Poison(),
            CardEffectType.Reroll => Reroll(),
            CardEffectType.StoreEnergy => StoreEnergy(),
            _ => null,
        };
    }
}
