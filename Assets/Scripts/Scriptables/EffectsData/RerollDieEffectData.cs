using UnityEngine;

[CreateAssetMenu(fileName = "RerollDieEffectData", menuName = "Cards/EffectData/RerollDie")]
public class RerollDieEffectData : EffectData
{
    public override EffectRuntime CreateRuntimeEffect()
    {
        return new RerollDiceEffectRuntime();
    }
}
