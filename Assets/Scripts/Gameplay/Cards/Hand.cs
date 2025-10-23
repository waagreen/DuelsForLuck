using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private CardLayout layout;
    [SerializeField] private CardVisual visualPrefab;
    [SerializeField] private int debugCards = 6;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnCreateDeck>(PopulateHand);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnCreateDeck>(PopulateHand);
    }

    // private void Start()
    // {
    //     if (debugCards > 6)
    //     {
    //         for (int i = 0; i < debugCards; i++)
    //         {
    //             CreateCard(new(), (i + 1) != debugCards);
    //         }
    //     }
    // }

    private void CreateCard(CardRuntime info, bool ignoreUpdate)
    {
        CardVisual cardVisual = Instantiate(visualPrefab, layout.transform);
        cardVisual.Setup(info);
        layout.AddCard(cardVisual, ignoreUpdate);
    }

    private void PopulateHand(OnCreateDeck evt)
    {
        List<CardRuntime> deck = evt.deck;

        if (deck == null || deck.Count < 1) return;

        for (int i = 0; i < deck.Count; i++)
        {
            CardRuntime cardInfo = deck[i];
            CreateCard(cardInfo, (i + 1) != deck.Count);
        }
    }

}
