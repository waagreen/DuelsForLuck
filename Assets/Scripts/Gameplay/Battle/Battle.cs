using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DieRoll
{
    public int value;
    public int damage;
}

public class Battle : MonoBehaviour
{
    [SerializeField] private int initalPlayersHealth = 10, initialDiceAmount = 2;
    [SerializeField] private StartingDeck testDeck;

    private Actor p1 = null;
    private Actor p2 = null;
    private int turnIndex;
    private bool isVisualsPlaying = false;
    private List<DieRoll> turnResults;

    private Actor GetActiveActor() => (turnIndex == 0) ? p1 : p2;
    private Actor GetPassiveActor() => (turnIndex == 0) ? p2 : p1;
    private int GetDamage(int value)
    {
        if (value == 1) return 0;
        else if (value == 6) return 2;
        else return 1;
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.AddSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.AddSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.RemoveSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.RemoveSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
    }

    private void Start()
    {
        turnIndex = 0;
        turnResults = new();
        
        p1 = new(turnIndex, initalPlayersHealth, initialDiceAmount, false, testDeck);
        p2 = new(turnIndex + 1, initalPlayersHealth, initialDiceAmount, true, testDeck);

        EventsManager.Broadcast(new OnCreateActor() { actor = p1 });
        EventsManager.Broadcast(new OnCreateActor() { actor = p2 });

        BroadcastTurnStart();
    }

    private void OnVisualsComplete(OnTurnVisualsComplete evt)
    {
        isVisualsPlaying = false;
    }

    private void ResetForNextRound(OnNextRound evt)
    {
        p1.Health = initalPlayersHealth;
        p2.Health = initalPlayersHealth;

        EventsManager.Broadcast(new OnActorHealthChange { dealtaHealth = initalPlayersHealth });
    }

    private void HandleDieResult(OnDieResult evt)
    {
        DieRoll roll = new()
        {
            value = evt.result,
            damage = GetDamage(evt.result)
        };
        turnResults.Add(roll);

        if (turnResults.Count != GetActiveActor().DiceAmount) return;

        Actor opposing = GetPassiveActor();
        foreach (DieRoll value in turnResults)
        {
            opposing.Health -= value.damage;
        }

        StartCoroutine(ResolveTurnSequence());
    }

    private IEnumerator ResolveTurnSequence()
    {
        isVisualsPlaying = true;

        // Broadcast event to start visual director
        EventsManager.Broadcast(new OnTurnResolveBegin() 
        { 
            results = new List<DieRoll>(turnResults),
            activeActor = GetActiveActor(),
            passiveActor = GetPassiveActor()
        });
        
        turnResults.Clear();

        // Wait until turn result visuals are done playing
        yield return new WaitUntil(() => isVisualsPlaying == false);

        // Resume turn logic
        turnIndex = (turnIndex + 1) % 2;

        if (p1.Health <= 0 || p2.Health <= 0)
        {
            CheckGameStatus();
        }
        else
        {
            BroadcastTurnStart();
            isVisualsPlaying = true;
        }

        // Wait until camera is positioned for play
        yield return new WaitUntil(() => isVisualsPlaying == false);
        BroadcastAviablePlay();
    }

    private void BroadcastTurnStart() => EventsManager.Broadcast(new OnTurnStart() { actor = GetActiveActor() });
    private void BroadcastAviablePlay() => EventsManager.Broadcast(new OnPlayIsAviable() { actor = GetActiveActor() });

    // Only called when one of the actors has taken fatal damage
    private void CheckGameStatus()
    {
        if (p1.IsWinner() || p2.IsWinner())
        {
            EventsManager.Broadcast(new OnGameEnd { actor = p1.IsWinner() ? p1 : p2 });
        }
        else
        {
            // No one reached 2 wins, we go the the next round.
            p1.Health = initalPlayersHealth;
            p2.Health = initalPlayersHealth;

            Actor winner = p1.Health <= 0 ? p2 : p1;
            winner.WinRound();

            EventsManager.Broadcast(new OnRoundEnd());
        }
    }
}
