using UnityEngine;

public class DamageEffectRuntime : EffectRuntime
{
    public int damage;
    public override void Execute()
    {
        Debug.Log($"{GetType()} <color=#F53229>NOT IMPLEMENTED YET!</color>");
    }
}
