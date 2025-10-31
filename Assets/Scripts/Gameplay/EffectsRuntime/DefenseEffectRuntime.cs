using UnityEngine;

public class DefenseEffectRuntime : EffectRuntime
{
    public int defense;
    public override void Execute(Actor self, Actor other)
    {
        self.ApplyStatus(CardEffectType.Armor, defense);
        EventsManager.Broadcast(new OnCardEffect
        {
            effectType = CardEffectType.Armor,
            value = defense,
            target = self,
            targetType = CardEffectTarget.Self
        });
    }
}
