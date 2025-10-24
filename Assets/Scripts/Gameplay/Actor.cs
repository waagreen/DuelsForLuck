using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Actor
{    
    private int health, wins, turnOrder, diceAmount;
    private bool isBot;
    private List<CardRuntime> deck;
    private List<CardRuntime> hand;
    private List<CardRuntime> discard;

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
        discard = new();

        if (startingDeck != null)
        {
            // Populate initial deck
            foreach (DeckEntry entry in startingDeck.cards)
            {
                for (int i = 0; i < entry.amount; i++)
                {
                    deck.Add(new CardRuntime(entry.data));
                }
            }
            
            // Deal inital hand
            for (int i = 0; i < kInitialHandSize; i++)
            {
                int randomIndex = Random.Range(0, deck.Count);
                CardRuntime randomCard = deck[randomIndex];

                deck.Remove(randomCard);
                hand.Add(randomCard);
            }
        }
    }

    public int Health
    {
        get => health;
        set => health = value;
    }
    
    public int DiceAmount => diceAmount;
    public int Order => turnOrder;
    public bool IsBot => isBot;
    
    public bool IsWinner() => wins == 2;
    public void WinRound()
    {
        wins++;
    }

    public List<CardRuntime> Deck => deck;
}
