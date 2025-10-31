using UnityEngine;

public class PrepareEffectRuntime : EffectRuntime
{
    public override void Execute(Actor self, Actor other)
    {
        self.ApplyStatus(CardEffectType.StoreEnergy, 1);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.StoreEnergy,
            value = 1,
            target = self,
            targetType = CardEffectTarget.Self
        });
    }
}
