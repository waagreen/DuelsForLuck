using System;
using System.Collections.Generic;
using UnityEngine;

public class CardRuntime
{
    private string nickname;
    private string description;
    private int cost;
    private Sprite artwork;
    private List<EffectRuntime> currentEffects;

    public string Nickname => nickname;
    public string Description => description;
    public int Cost => cost;
    public Sprite Artwork => artwork;
    public List<EffectRuntime> CurrentEffects => currentEffects;

    public Action<CardLayout> OnMove;

    public CardRuntime(CardData data)
    {
        nickname = data.nickname;
        description = data.description;
        artwork = data.artwork;
        cost = data.baseCost;

        currentEffects = new();

        if (data.effects == null) return;
        foreach (EffectData effect in data.effects)
        {
            currentEffects.Add(effect.CreateRuntimeEffect());
        }
    }

    public void OnPlay(Actor self, Actor other)
    {
        foreach (EffectRuntime effect in currentEffects)
        {
            effect.Execute(self, other);
        }
    }
}
