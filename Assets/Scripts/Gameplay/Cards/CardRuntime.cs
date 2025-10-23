using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRuntime
{
    private string nickname;
    private string description;
    private Sprite artwork;
    private List<EffectRuntime> currentEffects;

    public string Nickname => nickname;
    public string Description => description;
    public Sprite Artwork => artwork;
    public List<EffectRuntime> CurrentEffects => currentEffects;

    public CardRuntime(CardData data)
    {
        nickname = data.nickname;
        description = data.description;
        artwork = data.artwork;

        currentEffects = new();

        if (data.effects == null) return;
        foreach (EffectData effect in data.effects)
        {
            currentEffects.Add(effect.CreateRuntimeEffect());
        }
    }

    private void OnPlay()
    {
        foreach (EffectRuntime effect in currentEffects)
        {
            effect.Execute();
        }
    }
}
