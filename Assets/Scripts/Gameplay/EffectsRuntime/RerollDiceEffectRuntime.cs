using UnityEngine;

public class RerollDiceEffectRuntime : EffectRuntime
{
    public override void Execute(Actor self, Actor other)
    {
        self.ApplyStatus(CardEffectType.Reroll, 1);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.Reroll,
            value = 1,
            target = self,
            targetType = CardEffectTarget.Self
        });
    }
}
