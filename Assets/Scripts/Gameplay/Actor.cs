using System.Collections.Generic;
using UnityEngine;

public class Actor
{    
    private int health, wins, turnOrder, diceAmount;
    private bool isBot;
    private List<CardRuntime> deck;

    public static Color PColor => new(0.23f, 0.54f, 0.8f);
    public static Color BotColor => new(0.8f, 0.23f, 0.54f);

    public Actor(int turnOrder, int health, int diceAmount, bool isBot)
    {
        this.turnOrder = turnOrder;
        this.diceAmount = diceAmount;
        this.health = health;
        this.isBot = isBot;

        deck = new();
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
