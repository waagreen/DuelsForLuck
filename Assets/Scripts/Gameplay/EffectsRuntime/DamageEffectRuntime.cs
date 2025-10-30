using UnityEngine;

public class DamageEffectRuntime : EffectRuntime
{
    public int damage;
    public override void Execute(Actor self, Actor other)
    {
        other.UpdateHealth(-damage);
        EventsManager.Broadcast(new OnEffectVisualsTrigger
        {
            effectType = CardEffectType.Damage,
            value = -damage,
            target = other
        });
    }
}
