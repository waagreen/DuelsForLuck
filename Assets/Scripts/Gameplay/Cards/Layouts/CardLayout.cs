using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    [Header("Base Card Layout")]
    [SerializeField][Min(0.01f)] protected float updateDuration = 0.3f;
    [SerializeField] private bool isPickableLayout;
    
    protected const float kIndexOffsetZ = 0.001f;
    protected List<CardVisual> cards;
    protected Sequence updateSeq;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        cards = new();
    }

    protected virtual void OnDestroy()
    {
        
    }

    public virtual void UpdateLayout() { }

    public virtual void AddCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && !cards.Contains(card))
        {
            cards.Add(card);
            card.transform.SetParent(transform);
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    public virtual void RemoveCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && cards.Remove(card))
        {
            card.transform.SetParent(null);
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    private void OnCardEnter() {}
    private void OnCardExit() {} 
}
