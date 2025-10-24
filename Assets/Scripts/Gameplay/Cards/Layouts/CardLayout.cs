using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    [Header("Base Card Layout")]
    [SerializeField][Min(0.01f)] protected float updateDuration = 0.3f;
    
    protected const float kIndexOffsetZ = 0.001f;
    protected List<CardVisual> cards;
    protected Sequence updateSeq;

    protected virtual void Start()
    {
        Debug.Log("Initializing Layout");
        cards = new();
    }

    public virtual void UpdateLayout()
    {

    }

    public virtual void AddCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && !cards.Contains(card))
        {
            cards.Add(card);
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    public virtual void RemoveCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && cards.Remove(card))
        {
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    private void OnCardEnter() {}
    private void OnCardExit() {} 
}
