using System.Collections.Generic;
using UnityEngine;

public class DicePlayer : MonoBehaviour
{
    [SerializeField] private List<Die> dice;
    
    [Header("Explosion Settings")]
    [SerializeField][Min(0f)] private float radius = 5f;
    [SerializeField][Min(0f)] private float power = 10f, upwardsModifier = 10f;
    [SerializeField][Range(0f, 1f)] float maxRollTorque = 1f;

    private List<Vector3> diceInitialPosition;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnPlayIsAviable>(Play);
        EventsManager.AddSubscriber<OnTurnStart>(ResetPosition);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnPlayIsAviable>(Play);
        EventsManager.RemoveSubscriber<OnTurnStart>(ResetPosition);
    }

    private void Start()
    {
        dice ??= new();
        diceInitialPosition = new();

        foreach (Die die in dice)
        {
            diceInitialPosition.Add(die.transform.position);
        }
    }

    private void ResetPosition(OnTurnStart evt)
    {
        for (int i = 0; i < dice.Count; i++)
        {
            dice[i].transform.position = diceInitialPosition[i];
        }
    }

    private void Play(OnPlayIsAviable evt)
    {
        foreach (Die die in dice)
        {
            Rigidbody rb = die.Rb;
            rb.linearVelocity = Vector3.one * 0.01f;
            rb.AddExplosionForce(power, transform.position, radius, upwardsModifier, ForceMode.Impulse);
            die.Throw(maxRollTorque);
        }
    }
}
