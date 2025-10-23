using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEffectData", menuName = "Cards/EffectData/Poison")]
public class PoisonEffectData : EffectData
{
    public int baseDuration;
    public override EffectRuntime CreateRuntimeEffect()
    {
        return new PoisonEffectRuntime()
        {
            duration = baseDuration
        };
    }
}
