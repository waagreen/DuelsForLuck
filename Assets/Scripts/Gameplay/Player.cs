using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private DiceManipulator manipulator;
    private int health;
    private int wins;

    private readonly int diceAmount;
    private readonly Order order;

    public static Color P1Color => new(0.23f, 0.54f, 0.8f);
    public static Color P2Color => new(0.8f, 0.23f, 0.54f);

    public Player(Order order, int health, int diceAmount)
    {
        this.order = order;
        this.diceAmount = diceAmount;
        this.health = health;
    }

    public bool WinnedTheGame
    {
        get => wins == 2;
    }

    public int DiceAmount
    {
        get => diceAmount;
    }

    public int Health
    {
        get => health;
        set => health = value;
    }
    
    public Order Order
    {
        get => order;
    }

    public void WinRound()
    {
        wins++;        
    }
}
