using UnityEngine;

[CreateAssetMenu(fileName = "HealEffectData", menuName = "Cards/EffectData/Heal")]
public class HealEffectData : EffectData
{
    public int baseHeal;
    public override EffectRuntime CreateRuntimeEffect()
    {
        return new HealEffectRuntime()
        {
            heal = baseHeal
        };
    }
}
