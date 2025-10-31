using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Battle : MonoBehaviour
{
    [SerializeField] private int initalPlayersHealth = 10, drawCardsPerTurn = 3;
    [SerializeField] private StartingDeck testDeck;

    private Actor p1 = null, p2 = null;
    private int turnIndex, picksThisTurn;
    private bool isVisualDirectorPlaying = false;

    private Actor GetActiveActor() => (turnIndex == 0) ? p1 : p2;
    private Actor GetPassiveActor() => (turnIndex == 0) ? p2 : p1;
    private WaitUntil WaitVisualDirector() => new(() => isVisualDirectorPlaying == false);

    private void OnVisualsComplete(OnTurnVisualsComplete evt)
    {
        isVisualDirectorPlaying = false;
    }

    private void ResetForNextRound(OnNextRound evt)
    {
        p1.ResetHealth();
        p2.ResetHealth();
    }

    private void ResolveCardPlay(OnPlayCard evt)
    {
        Actor actor = GetActiveActor();
        
        actor.Hand.Remove(evt.card);
        actor.Discard.Add(evt.card);
        evt.card.OnPlay(actor, GetPassiveActor());

        EventsManager.Broadcast(new OnSendCardsToDiscard { cards = new() { evt.card }, ownerOrder = turnIndex });
    }

    private void ResolveDrawPick(OnPickDrawCard evt)
    {
        picksThisTurn++;
        Actor actor = GetActiveActor();

        // Remove selected card from draw pool
        actor.Draw.Remove(evt.card);
        EventsManager.Broadcast(new OnSendCardsToHand { cards = new() { evt.card }, ownerOrder = turnIndex });

        if (picksThisTurn < actor.TurnPickAmount) return;

        // Discard remaining cards on the draw pool, then clear it
        List<CardRuntime> remainingCards = actor.Draw.ToList();
        actor.Discard.AddRange(remainingCards);
        actor.Draw.Clear();

        // Indicates that all possible cards have been picked, and the turn gameplay can commence
        EventsManager.Broadcast(new OnSendCardsToDiscard { cards = remainingCards, ownerOrder = turnIndex });
        EventsManager.Broadcast(new OnPlayIsAviable() { actor = GetActiveActor() });
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
        isVisualDirectorPlaying = true;
        EventsManager.Broadcast(new OnTurnStart() { actor = GetActiveActor() });
        
        // Wait until camera is positioned for play
        yield return WaitVisualDirector();

        picksThisTurn = 0;
        DrawTurnCards();
    }

    private IEnumerator ResolveTurn()
    {
        isVisualDirectorPlaying = true;

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
            Actor winner = p1.Health <= 0 ? p2 : p1;
            winner.WinRound();

            EventsManager.Broadcast(new OnRoundEnd { actor = winner });
        }
    }

    private void Awake()
    {
        EventsManager.AddSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.AddSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
        EventsManager.AddSubscriber<OnPickDrawCard>(ResolveDrawPick);
        EventsManager.AddSubscriber<OnPlayCard>(ResolveCardPlay);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnNextRound>(ResetForNextRound);
        EventsManager.RemoveSubscriber<OnTurnVisualsComplete>(OnVisualsComplete);
        EventsManager.RemoveSubscriber<OnPickDrawCard>(ResolveDrawPick);
        EventsManager.RemoveSubscriber<OnPlayCard>(ResolveCardPlay);
}

    private void Start()
    {
        turnIndex = 0;
        
        p1 = new(turnIndex, initalPlayersHealth, false, testDeck);
        p2 = new(turnIndex + 1, initalPlayersHealth, true, testDeck);

        // Broadcast newly created players and decks
        EventsManager.Broadcast(new OnCreateActor { actor = p1 });
        EventsManager.Broadcast(new OnCreateActor { actor = p2 });

        StartCoroutine(InitializeTurn());
    }
}
