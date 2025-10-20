using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum Order
{
    First = 0,
    Second = 1
}

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

    private Player p1 = null;
    private Player p2 = null;
    private Order turn;
    private List<DieRoll> turnResults;

    private Sequence turnResolveSeq;

    private Player GetActivePlayer() => (turn == Order.First) ? p1 : p2;
    private Player GetOpposingPlayer() => (turn == Order.First) ? p2 : p1;
    private int GetDamage(int value)
    {
        if (value == 1) return 0;
        else if (value == 6) return 2;
        else return 1;
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.AddSubscriber<OnResetTurn>(ResetTurnFromLoser);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.RemoveSubscriber<OnResetTurn>(ResetTurnFromLoser);
    }
    
    private void Start()
    {
        p1 = new(Order.First, initalPlayersHealth, initialDiceAmount);
        p2 = new(Order.Second, initalPlayersHealth, initialDiceAmount);

        turnResults = new();
        InitTurns(Order.First);

        EventsManager.Broadcast(new OnCreatePlayer() {newPlayer = p1});
        EventsManager.Broadcast(new OnCreatePlayer() {newPlayer = p2});
    }

    private void CheckGameStatus()
    {
        if (p1.WinnedTheGame || p2.WinnedTheGame)
        {
            EventsManager.Broadcast(new OnGameEnd { winner = p1.WinnedTheGame ? p1 : p2 });
        }
        else
        {
            // No one reached 2 wins, we go the the next round.

            p1.Health = initalPlayersHealth;
            p2.Health = initalPlayersHealth;

            Player winner = p1.Health <= 0 ? p2 : p1;
            winner.WinRound();

            // At this point the turn already progressed, so the winner is the previous player
            turnResolveSeq.AppendCallback(() =>
            {
                EventsManager.Broadcast(new OnRoundEnd { winner = turn == Order.First ? Order.Second : Order.First });
            });
            turnResolveSeq.Play();
        }
    }

    private void HandleDieResult(OnDieResult evt)
    {
        DieRoll roll = new()
        {
            value = evt.result,
            damage = GetDamage(evt.result)
        };
        turnResults.Add(roll);

        if (turnResults.Count != GetActivePlayer().DiceAmount) return;

        Player opposing = GetOpposingPlayer();
        foreach (DieRoll value in turnResults)
        {
            opposing.Health -= value.damage;
        }

        ResolveTurn();
    }

    private void BroadcastTurn()
    {
        EventsManager.Broadcast(new OnTurnChange() { turnOrder = turn });
    }

    private void TurnResolveFeedback()
    {
        // This represents the player who will play this new turn 
        BattleDisplay battleDisplay = turn == Order.First ? p1Display : p2Display;

        turnResolveSeq?.Kill();
        turnResolveSeq = DOTween.Sequence();

        // Look to the opposing player
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(0f, 0.6f).SetEase(Ease.OutBack));
        turnResolveSeq.AppendCallback(BroadcastTurn);
        turnResolveSeq.AppendInterval(0.5f);

        // Show the dice results 
        turnResolveSeq.Append(battleDisplay.DamageFeedbackSequence(turnResults, turn));
    }
    
    private void NextTurnFeedback()
    {
        float orbitAngle = turn == Order.First ? 180f : 0f;

        // Camera moves to the perspective of next player
        turnResolveSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1f).SetEase(Ease.InOutSine));
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.6f).SetEase(Ease.InBack));
        turnResolveSeq.Play();
    }

    private void ResolveTurn()
    {
        // Toggle between players
        turn = (Order)(((int)turn + 1) % 2);

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

    private void ResetTurnFromLoser(OnResetTurn evt)
    {
        EventsManager.Broadcast(new OnPlayerHealthChange { dealtaHealth = initalPlayersHealth });
        InitTurns(turn);
    }

    private void InitTurns(Order forceOrder)
    {
        // Enforce the current player
        turn = forceOrder;
        float orbitAngle = turn == Order.First ? 180f : 0f;

        turnResolveSeq?.Kill();
        turnResolveSeq = DOTween.Sequence();
        turnResolveSeq.AppendCallback(BroadcastTurn);
        turnResolveSeq.AppendInterval(1f);
        turnResolveSeq.Append(orbitCamera.OrbitToAngle(orbitAngle, 1.5f).SetEase(Ease.InOutSine));
        turnResolveSeq.AppendInterval(0.2f);
        turnResolveSeq.Append(orbitCamera.AdjustViewAngle(45f, 0.7f).SetEase(Ease.OutBack));
        turnResolveSeq.Play();
    }
}
