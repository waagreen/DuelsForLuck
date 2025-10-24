using System.Collections.Generic;
using UnityEngine;

public class HandVisual : MonoBehaviour
{
    [SerializeField] private HorizontalCardLayout layout;
    [SerializeField] private int ownerStartingOrder;

    private void Awake()
    {
        EventsManager.AddSubscriber<OnDrawCard>(ReceiveCard);
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDrawCard>(ReceiveCard);
    }

    private void ReceiveCard(OnDrawCard evt)
    {
    }
}
