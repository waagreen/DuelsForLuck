using UnityEngine;

public class HandVisual : MonoBehaviour
{
    [SerializeField] private HorizontalCardLayout layout;
    [SerializeField] private int ownerStartingOrder;

    public HorizontalCardLayout Layout => layout;
    private bool isActive; // Only indicates if the cards state will change, not if the hand is interactable

    private void Awake()
    {
        EventsManager.AddSubscriber<OnTurnStart>(UpdateHandState);
        EventsManager.AddSubscriber<OnSelectDie>(UpdateCardsCostState);
        EventsManager.AddSubscriber<OnDisselectDie>(ResetCostAndDisableCards);

    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnTurnStart>(UpdateHandState);
        EventsManager.RemoveSubscriber<OnSelectDie>(UpdateCardsCostState);
        EventsManager.RemoveSubscriber<OnDisselectDie>(ResetCostAndDisableCards);
    }

    private void ResetCostAndDisableCards(OnDisselectDie evt)
    {
        foreach (CardVisual card in layout.Cards)
        {
            card.UpdateActiveState(false);
            card.UpdateCostState(-1);
        }
    }

    private void UpdateHandState(OnTurnStart evt)
    {
        if (layout.IsPickable) return;

        // Disable cards on turn start
        isActive = evt.actor.Order == ownerStartingOrder;
        foreach (CardVisual card in layout.Cards)
        {
            card.UpdateActiveState(false);
        }
    }

    private void UpdateCardsCostState(OnSelectDie evt)
    {
        if (!isActive) return;
        
        foreach (CardVisual card in layout.Cards)
        {
            card.UpdateCostState(evt.value);
        }
    }
}
