using UnityEngine;

public class DamageEffectRuntime : EffectRuntime
{
    public int damage;
    public override void Execute(Actor self, Actor other)
    {
        other.UpdateHealth(-damage);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.Damage,
            value = -damage,
            target = other,
            targetType = CardEffectTarget.Other
        });
    }
}
