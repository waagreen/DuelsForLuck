using System.Collections.Generic;
using UnityEngine;

public class Actor
{    
    private int health, wins, turnOrder, diceAmount;
    private bool isBot;
    private List<CardRuntime> deck;

    public static Color PColor => new(0.23f, 0.54f, 0.8f);
    public static Color BotColor => new(0.8f, 0.23f, 0.54f);

    public Actor(int turnOrder, int health, int diceAmount, bool isBot, StartingDeck startingDeck)
    {
        this.turnOrder = turnOrder;
        this.diceAmount = diceAmount;
        this.health = health;
        this.isBot = isBot;

        deck = new();
        if (startingDeck != null)
        {
            foreach (DeckEntry entry in startingDeck.cards)
            {
                for (int i = 0; i < entry.amount; i++)
                {
                    deck.Add(new CardRuntime(entry.data));
                }
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
}
