using UnityEngine;

[CreateAssetMenu(fileName = "PrepareEffectData", menuName = "Cards/EffectData/Prepare")]
public class PrepareEffectData : EffectData
{
    public override EffectRuntime CreateRuntimeEffect()
    {
        return new PrepareEffectRuntime();
    }
}
