using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public struct DieRoll
{
    public int value;
    public int damage;
}

public class Battle : MonoBehaviour
{
    [SerializeField] private OrbitCamera orbitCamera;
    [SerializeField] private BattleDisplay p1Display, p2Display;
    [SerializeField] private int initalPlayersHealth = 10, initialDiceAmount = 2;

    private Actor p1 = null;
    private Actor p2 = null;
    private int turnIndex;
    private List<DieRoll> turnResults;

    private Sequence turnResolveSeq;

    private Actor GetActiveActor() => (turnIndex == 0) ? p1 : p2;
    private Actor GetOpposingActor() => (turnIndex == 1) ? p2 : p1;
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
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.RemoveSubscriber<OnNextRound>(ResetForNextRound);
    }

    private void Start()
    {
        turnIndex = 0;
        turnResults = new();
        
        p1 = new(turnIndex, initalPlayersHealth, initialDiceAmount, false);
        p2 = new(turnIndex + 1, initalPlayersHealth, initialDiceAmount, true);

        InitTurns(turnIndex);

        EventsManager.Broadcast(new OnCreateActor() { newActor = p1 });
        EventsManager.Broadcast(new OnCreateActor() { newActor = p2 });
    }

    private void InitTurns(int forceOrder)
    {
        // Enforce the current player
        turnIndex = forceOrder;
        float orbitAngle = (turnIndex == 0) ? 180f : 0f;

        turnResolveSeq?.Kill();
        turnResolveSeq = DOTween.Sequence();
        turnResolveSeq.AppendCallback(BroadcastTurnChange);
        turnResolveSeq.AppendInterval(1f);
        turnResolveSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1.5f).SetEase(Ease.InOutSine));
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.7f).SetEase(Ease.OutBack));
        turnResolveSeq.Play();
    }

    private void ResetForNextRound(OnNextRound evt)
    {
        EventsManager.Broadcast(new OnActorHealthChange { dealtaHealth = initalPlayersHealth });
        InitTurns(turnIndex);
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

        Actor opposing = GetOpposingActor();
        foreach (DieRoll value in turnResults)
        {
            opposing.Health -= value.damage;
        }

        ResolveTurn();
    }

    private void ResolveTurn()
    {
        // Toggle between players
        turnIndex = (turnIndex + 1) % 2;

        TurnResolveFeedback();
        turnResults.Clear();

        if (p1.Health <= 0 || p2.Health <= 0)
        {
            CheckGameStatus();
        }
        else
        {
            NextTurnFeedback();
        }
    }

    private void BroadcastTurnChange() => EventsManager.Broadcast(new OnImmediateTurnChange() { turnIndex = turnIndex });
    private void BroadcastTurnStart() => EventsManager.Broadcast(new OnTurnStart() { currentActor = GetActiveActor() });

    // Always called between turns to display the dice results
    private void TurnResolveFeedback()
    {
        // This represents the actor who will play this new turn 
        BattleDisplay battleDisplay = (turnIndex == 0) ? p1Display : p2Display;

        turnResolveSeq?.Kill();
        turnResolveSeq = DOTween.Sequence();

        // Look to the opposing actor
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(0f, 0.6f).SetEase(Ease.OutBack));
        turnResolveSeq.AppendCallback(BroadcastTurnChange);
        turnResolveSeq.AppendInterval(0.5f);

        // Show the dice results 
        turnResolveSeq.Append(battleDisplay.DamageFeedbackSequence(turnResults, turnIndex));
    }
    
    // Only called when both actors have some health left
    private void NextTurnFeedback()
    {
        float orbitAngle = (turnIndex == 0) ? 180f : 0f;

        // Camera moves to the perspective of next player
        turnResolveSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1f).SetEase(Ease.InOutSine));
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.6f).SetEase(Ease.InBack));
        turnResolveSeq.AppendCallback(BroadcastTurnStart);
        turnResolveSeq.Play();
    }

    // Only called when one of the actors has taken fatal damage
    private void CheckGameStatus()
    {
        if (p1.IsWinner() || p2.IsWinner())
        {
            EventsManager.Broadcast(new OnGameEnd { winner = p1.IsWinner() ? p1 : p2 });
        }
        else
        {
            // No one reached 2 wins, we go the the next round.
            p1.Health = initalPlayersHealth;
            p2.Health = initalPlayersHealth;

            Actor winner = p1.Health <= 0 ? p2 : p1;
            winner.WinRound();

            // At this point the turn already progressed, so the winner is the previous player
            turnResolveSeq.AppendCallback(() =>
            {
                EventsManager.Broadcast(new OnRoundEnd { winner = (turnIndex + 1) % 2 });
            });
            turnResolveSeq.Play();
        }
    }
}
