using UnityEngine;

public class PoisonEffectRuntime : EffectRuntime
{
    public int duration;
    public override void Execute()
    {
        Debug.Log($"{GetType()} <color=#F53229>NOT IMPLEMENTED YET!</color>");
    }
}
