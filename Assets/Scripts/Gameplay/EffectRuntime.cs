using UnityEngine;

[System.Serializable]
public abstract class EffectRuntime
{
    public int cost;

    public abstract void Execute(Actor self, Actor other);
}
