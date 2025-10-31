using UnityEngine;

public class HealEffectRuntime : EffectRuntime
{
    public int heal;
    public override void Execute(Actor self, Actor other)
    {
        self.UpdateHealth(heal);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.Heal,
            value = heal,
            target = self,
            targetType = CardEffectTarget.Self
        });
    }
}
