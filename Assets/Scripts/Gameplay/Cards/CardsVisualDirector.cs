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
        EventsManager.AddSubscriber<OnSendDeck>(SendToDeck);
        EventsManager.AddSubscriber<OnSendDraw>(SendToDraw);
        EventsManager.AddSubscriber<OnSendDiscard>(SendToDiscard);
        EventsManager.AddSubscriber<OnSendHand>(SendToHand);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateActor>(DealInitialCards);
        EventsManager.RemoveSubscriber<OnSendDeck>(SendToDeck);
        EventsManager.RemoveSubscriber<OnSendDraw>(SendToDraw);
        EventsManager.RemoveSubscriber<OnSendDiscard>(SendToDiscard);
        EventsManager.RemoveSubscriber<OnSendHand>(SendToHand);
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

    private void InvokeGoTo(List<CardRuntime> cards, Transform destination, CardLayout layout)
    {
        foreach (CardRuntime card in cards)
        {
            card.OnMove?.Invoke(destination.position, layout);
        }
    }

    private void SendToDeck(OnSendDeck evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, deck.transform, deck.Layout);
    }

    private void SendToDiscard(OnSendDiscard evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, discard.transform, discard.Layout);
    }

    private void SendToHand(OnSendHand evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, hand.transform, hand.Layout);
    }
    
    private void SendToDraw(OnSendDraw evt)
    {
        if (!IsOwnerActor(evt.ownerOrder)) return;
        InvokeGoTo(evt.cards, draw.transform, draw.Layout);
    }
}
