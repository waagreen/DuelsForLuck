using System.Collections.Generic;
using UnityEngine;

public class DeckVisual : MonoBehaviour
{
    [SerializeField] private StackCardLayout layout;
    [SerializeField] private CardVisual visualPrefab;
    [SerializeField] private int ownerStartingOrder;
    
    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateActor>(PopulateDeck);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateActor>(PopulateDeck);
    }

    private void CreateCard(CardRuntime info, bool ignoreUpdate)
    {
        CardVisual cardVisual = Instantiate(visualPrefab, layout.transform);
        cardVisual.Setup(info);
        layout.AddCard(cardVisual, ignoreUpdate);
    }

    private void PopulateDeck(OnCreateActor evt)
    {
        if (ownerStartingOrder != evt.actor.Order) return;
        
        List<CardRuntime> deck = evt.actor.Deck;
        int count = deck.Count;

        if (deck == null || count < 1) return;

        for (int i = 0; i < count; i++)
        {
            CardRuntime cardInfo = deck[i];
            CreateCard(cardInfo, (i + 1) != deck.Count);
        }
    }
}
