using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private CardLayout layout;
    [SerializeField] private CardVisual visualPrefab;
    [SerializeField] private int ownerStartingOrder;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateActor>(PopulateHand);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateActor>(PopulateHand);
    }

    private void CreateCard(CardRuntime info, bool ignoreUpdate)
    {
        CardVisual cardVisual = Instantiate(visualPrefab, layout.transform);
        cardVisual.Setup(info);
        layout.AddCard(cardVisual, ignoreUpdate);
    }

    private void PopulateHand(OnCreateActor evt)
    {
        if (ownerStartingOrder != evt.newActor.Order) return;
        
        List<CardRuntime> deck = evt.newActor.Deck;
        int count = deck.Count;

        if (deck == null || count < 1) return;

        for (int i = 0; i < count; i++)
        {
            CardRuntime cardInfo = deck[i];
            CreateCard(cardInfo, (i + 1) != deck.Count);
        }
    }

}
