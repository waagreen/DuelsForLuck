using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CardEffectType
{
    Damage,
    Armor,
    Heal,
    Poison,
    Reroll,
    StoreEnergy
}

public enum CardEffectTarget
{
    Self,
    Other,
    Both
}

public class Actor
{
    private int wins, turnOrder, turnDrawAmount;
    private int health, maxHealth;
    private bool isBot;
    private Dictionary<CardEffectType, int> activeStatuses;

    private readonly List<CardRuntime> deck;
    private readonly List<CardRuntime> hand;
    private readonly List<CardRuntime> discard;
    private readonly List<CardRuntime> draw;

    private const int kInitialHandSize = 5;

    public static Color PColor => new(0.23f, 0.54f, 0.8f);
    public static Color BotColor => new(0.8f, 0.23f, 0.54f);

    public Actor(int turnOrder, int health, bool isBot, StartingDeck startingDeck)
    {
        this.turnOrder = turnOrder;
        this.isBot = isBot;
        this.health = maxHealth = health;

        activeStatuses = new();
        deck = new();
        hand = new();
        draw = new();
        discard = new();

        turnDrawAmount = 1;

        if (startingDeck != null)
        {
            // Populate initial deck
            foreach (DeckEntry entry in startingDeck.cards)
            {
                for (int i = 0; i < entry.amount; i++)
                {
                    CardRuntime card = new(entry.data);
                    deck.Add(card);
                }
            }
            deck.Shuffle();

            // Deal inital hand
            List<CardRuntime> initialHand = deck.Take(kInitialHandSize).ToList();
            hand.AddRange(initialHand);
            deck.RemoveRange(0, kInitialHandSize);
        }
    }

    public void UpdateHealth(int delta)
    {
        health = Mathf.Clamp(health + delta, 0, maxHealth);
        EventsManager.Broadcast(new OnActorHealthChange { dealtaHealth = delta });
    }

    public void ResetHealth()
    {
        health = maxHealth;
        EventsManager.Broadcast(new OnActorHealthChange { dealtaHealth = maxHealth });
    }

    public void ApplyStatus(CardEffectType type, int amount)
    {
        if (activeStatuses.ContainsKey(type))
        {
            activeStatuses[type] += amount;
        }
        else activeStatuses.Add(type, amount);
    }

    public int GetStatus(CardEffectType type)
    {
        if (activeStatuses.TryGetValue(type, out int value))
        {
            return value;
        }
        else return 0;
    }

    public void SetStatus(CardEffectType type, int value)
    {
        activeStatuses[type] = value;     
    }
    
    public void ClearStatus(CardEffectType type)
    {
        if (activeStatuses.ContainsKey(type))
        {
            activeStatuses.Remove(type);
        }
    }

    public bool IsWinner() => wins == 2;
    public void WinRound() => wins++;

    public int Health => health;
    public int Order => turnOrder;
    public int TurnPickAmount => turnDrawAmount;
    public bool IsBot => isBot;

    public List<CardRuntime> Deck => deck;
    public List<CardRuntime> Draw => draw;
    public List<CardRuntime> Discard => discard;
    public List<CardRuntime> Hand => hand;
}
