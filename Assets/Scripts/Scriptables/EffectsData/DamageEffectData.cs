using UnityEngine;

[CreateAssetMenu(fileName = "EffectData", menuName = "Cards/EffectData/Damage")]
public class DamageEffectData : EffectData
{
    public int baseDamage;

    public override EffectRuntime CreateRuntimeEffect()
    {
        return new DamageEffectRuntime()
        {
            damage = baseDamage
        };
    }
}
