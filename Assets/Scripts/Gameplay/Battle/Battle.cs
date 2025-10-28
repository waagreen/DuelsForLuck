using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct DieRoll
{
    public int value;
    public int damage;
}

public class Battle : MonoBehaviour
{
    [SerializeField] private int initalPlayersHealth = 10, initialDiceAmount = 2, drawCardsPerTurn = 3;
    [SerializeField] private StartingDeck testDeck;

    private Actor p1 = null;
    private Actor p2 = null;
    private int turnIndex, picksThisTurn;
    private bool isVisualDirectorPlaying = false;
    private List<DieRoll> turnResults;

    private Actor GetActiveActor() => (turnIndex == 0) ? p1 : p2;
    private Actor GetPassiveActor() => (turnIndex == 0) ? p2 : p1;
    private int GetDamage(int value)
    {
        if (value == 1) return 0;
        else if (value == 6) return 2;
        else return 1;
    }

    private WaitUntil WaitVisualDirector() => new(() => isVisualDirectorPlaying == false);

    private void OnVisualsComplete(OnTurnVisualsComplete evt)
    {
        isVisualDirectorPlaying = false;
    }

    private void ResetForNextRound(OnNextRound evt)
    {
        p1.Health = initalPlayersHealth;
        p2.Health = initalPlayersHealth;

        EventsManager.Broadcast(new OnActorHealthChange { dealtaHealth = initalPlayersHealth });
    }

    private void ResolveDrawPick(OnPickDrawCard evt)
    {
        picksThisTurn++;
        Actor actor = GetActiveActor();

        // Remove selected card from draw pool
        actor.Draw.Remove(evt.pickedCard);
        EventsManager.Broadcast(new OnSendCardsToHand { cards = new(){evt.pickedCard}, ownerOrder = turnIndex });

        if (picksThisTurn < actor.TurnPickAmount) return;

        // Discard remaining cards on the draw pool, then clear it
        List<CardRuntime> remainingCards = actor.Draw.ToList();
        actor.Discard.AddRange(remainingCards);
        actor.Draw.Clear();

        EventsManager.Broadcast(new OnSendCardsToDiscard { cards = remainingCards, ownerOrder = turnIndex });
    }

    private void DrawTurnCards()
    {
        Actor activeActor = GetActiveActor();
        int cardsToDraw = Mathf.Min(drawCardsPerTurn, activeActor.Deck.Count);

        if (cardsToDraw < 1)
        {
            // TODO: Do shuffle discard into deck
            return;
        }

        List<CardRuntime> cardsToSend = activeActor.Deck.Take(drawCardsPerTurn).ToList();
        activeActor.Draw.AddRange(cardsToSend);
        activeActor.Deck.RemoveRange(0, cardsToDraw);

        OnSendCardsToDraw drawEvent = new()
        {
            cards = cardsToSend,
            ownerOrder = turnIndex
        };

        EventsManager.Broadcast(drawEvent);
    }

    private IEnumerator InitializeTurn()
    {
        // Wait until camera is positioned for play
        isVisualDirectorPlaying = true;
        BroadcastTurnStart();
        yield return WaitVisualDirector();

        picksThisTurn = 0;
        BroadcastAviablePlay();
        DrawTurnCards();
    }

    private IEnumerator ResolveTurn()
    {
        isVisualDirectorPlaying = true;

        // Broadcast event to start visual director
        EventsManager.Broadcast(new OnTurnResolveBegin()
        {
            results = new List<DieRoll>(turnResults),
            activeActor = GetActiveActor(),
            passiveActor = GetPassiveActor()
        });

        turnResults.Clear();

        // Wait until turn result visuals are done playing
        yield return WaitVisualDirector();

        // Resume turn logic
        turnIndex = (turnIndex + 1) % 2;

        if (p1.Health <= 0 || p2.Health <= 0)
        {
            CheckGameStatus();
        }
        else
        {
            StartCoroutine(InitializeTurn());
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

        if (turnResults.Count != GetActiveActor().DiceAmount) return;

        Actor opposing = GetPassiveActor();
        foreach (DieRoll value in turnResults)
        {
            opposing.Health -= value.damage;
        }

        StartCoroutine(ResolveTurn());
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

    private void Awake()
    {
        EventsManager.AddSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.AddSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.AddSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
        EventsManager.AddSubscriber<OnPickDrawCard>(ResolveDrawPick);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDieResult>(HandleDieResult);
        EventsManager.RemoveSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.RemoveSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
        EventsManager.RemoveSubscriber<OnPickDrawCard>(ResolveDrawPick);
    }

    private void Start()
    {
        turnIndex = 0;
        turnResults = new();
        
        p1 = new(turnIndex, initalPlayersHealth, initialDiceAmount, false, testDeck);
        p2 = new(turnIndex + 1, initalPlayersHealth, initialDiceAmount, true, testDeck);

        // Broadcast newly created players and decks
        EventsManager.Broadcast(new OnCreateActor { actor = p1 });
        EventsManager.Broadcast(new OnCreateActor { actor = p2 });

        StartCoroutine(InitializeTurn());
    }
}
