using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    public string effectName;
    [TextArea] public string effectDescription;

    public abstract EffectRuntime CreateRuntimeEffect();
}
