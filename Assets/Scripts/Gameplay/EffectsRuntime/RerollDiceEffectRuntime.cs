using UnityEngine;

public class RerollDiceEffectRuntime : EffectRuntime
{
    public override void Execute(Actor self, Actor other)
    {
        self.ApplyStatus(CardEffectType.Rerolls, 1);
        EventsManager.Broadcast(new OnEffectVisualsTrigger
        {
            effectType = CardEffectType.Rerolls,
            value = 1,
            target = self
        });
    }
}
