using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private CardLayout layout;
    [SerializeField] private CardVisual visualPrefab;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateDeck>(PopulateHand);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateDeck>(PopulateHand);
    }

    private void PopulateHand(OnCreateDeck evt)
    {
        List<CardRuntime> deck = evt.deck;

        if (deck == null || deck.Count < 1) return;
        
        for(int i = 0; i < deck.Count; i++)
        {
            CardRuntime cardInfo = deck[i];
            CardVisual cardVisual = Instantiate(visualPrefab, layout.transform);
            cardVisual.Setup(cardInfo);
            layout.AddCard(cardVisual, ignoreUpdate: (i + 1) != deck.Count);
        }
    }

}
