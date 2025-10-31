using UnityEngine;

public class PoisonEffectRuntime : EffectRuntime
{
    public int duration;
    public override void Execute(Actor self, Actor other)
    {
        other.ApplyStatus(CardEffectType.Poison, duration);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.Poison,
            value = duration,
            target = other,
            targetType = CardEffectTarget.Other
        });
    }
}
