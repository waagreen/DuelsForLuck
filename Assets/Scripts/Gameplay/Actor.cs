using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor
{    
    private int health, wins, turnOrder, diceAmount, turnDrawAmount;
    private bool isBot;
    private readonly List<CardRuntime> deck;
    private readonly List<CardRuntime> hand;
    private readonly List<CardRuntime> discard;
    private readonly List<CardRuntime> draw;

    private const int kInitialHandSize = 5;

    public static Color PColor => new(0.23f, 0.54f, 0.8f);
    public static Color BotColor => new(0.8f, 0.23f, 0.54f);

    public Actor(int turnOrder, int health, int diceAmount, bool isBot, StartingDeck startingDeck)
    {
        this.turnOrder = turnOrder;
        this.diceAmount = diceAmount;
        this.health = health;
        this.isBot = isBot;

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
                    CardRuntime card = new (entry.data);
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

    public int Health
    {
        get => health;
        set => health = value;
    }
    
    public int DiceAmount => diceAmount;
    public int Order => turnOrder;
    public int TurnPickAmount => turnDrawAmount;
    public bool IsBot => isBot;
    
    public bool IsWinner() => wins == 2;
    public void WinRound()
    {
        wins++;
    }

    public List<CardRuntime> Deck => deck;
    public List<CardRuntime> Draw => draw;
    public List<CardRuntime> Discard => discard;
    public List<CardRuntime> Hand => hand;
}
