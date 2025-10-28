using System.Collections.Generic;
using UnityEngine;

public class CardsVisualDirector : MonoBehaviour
{
    [SerializeField] private int ownerStartingOrder;
    [SerializeField] private CardVisual visualPrefab;
    [SerializeField] private HandVisual hand, draw;
    [SerializeField] private DeckVisual discard, deck;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateActor>(DealInitialCards);
        EventsManager.AddSubscriber<OnSendCardsToDeck>(SendToDeck);
        EventsManager.AddSubscriber<OnSendCardsToDraw>(SendToDraw);
        EventsManager.AddSubscriber<OnSendCardsToDiscard>(SendToDiscard);
        EventsManager.AddSubscriber<OnSendCardsToHand>(SendToHand);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateActor>(DealInitialCards);
        EventsManager.RemoveSubscriber<OnSendCardsToDeck>(SendToDeck);
        EventsManager.RemoveSubscriber<OnSendCardsToDraw>(SendToDraw);
        EventsManager.RemoveSubscriber<OnSendCardsToDiscard>(SendToDiscard);
        EventsManager.RemoveSubscriber<OnSendCardsToHand>(SendToHand);
    }

    private bool IsOwnerActor(int eventOrder) => eventOrder == ownerStartingOrder;

    private void CreateCards(List<CardRuntime> cardList, CardLayout layout)
    {
        int count = cardList.Count;
        if (cardList == null || count < 1) return;
        
        for (int i = 0; i < count; i++)
        {
            CardRuntime cardInfo = cardList[i];
            bool ignoreUpdate = (i + 1) != count;
            
            CardVisual cardVisual = Instantiate(visualPrefab, layout.transform);
            cardVisual.name = $"Card{ownerStartingOrder} - {cardInfo.Nickname}";
            cardVisual.Setup(cardInfo, layout);
            layout.AddCard(cardVisual, ignoreUpdate);
        }

    }

    private void DealInitialCards(OnCreateActor evt)
    {
        if (!IsOwnerActor(evt.actor.Order)) return;
        
        CreateCards(evt.actor.Deck, deck.Layout); // Create initial deck
        CreateCards(evt.actor.Hand, hand.Layout); // Create initial hand
    }

    private void InvokeGoTo(List<CardRuntime> cards, CardLayout layout)
    {
        foreach (CardRuntime card in cards)
        {
            card.OnMove?.Invoke(layout);
        }
    }

    private void SendToDeck(OnSendCardsToDeck evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, deck.Layout);
    }

    private void SendToDiscard(OnSendCardsToDiscard evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, discard.Layout);
    }

    private void SendToHand(OnSendCardsToHand evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, hand.Layout);
    }
    
    private void SendToDraw(OnSendCardsToDraw evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, draw.Layout);
    }
}
