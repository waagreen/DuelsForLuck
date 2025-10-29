using UnityEngine;

public class DefenseEffectRuntime : EffectRuntime
{
    public int defense;
    public override void Execute()
    {
        Debug.Log($"{GetType()} <color=#F53229>NOT IMPLEMENTED YET!</color>");
    }
}
