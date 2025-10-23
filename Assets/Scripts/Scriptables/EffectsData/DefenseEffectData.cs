using UnityEngine;

[CreateAssetMenu(fileName = "DefenseEffectData", menuName = "Cards/EffectData/Defense")]
public class DefenseEffectData : EffectData
{
    public int baseDefense;

    public override EffectRuntime CreateRuntimeEffect()
    {
        return new DefenseEffectRuntime()
        {
            defense = baseDefense
        };
    }
}
