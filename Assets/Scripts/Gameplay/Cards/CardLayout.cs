using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardLayout : MonoBehaviour
{
    [SerializeField] private Vector2 xyBounds;

    [SerializeField][Min(0.01f)] private float updateDuration = 0.3f;
    [SerializeField][Min(0f)] private float spacing = 0.5f;
    [SerializeField] private float arcDeggres = 15f;

    private const float kIndexOffsetZ = 0.05f;
    private Sequence updateSeq;
    private List<CardVisual> cards;

    private void Start()
    {
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        if ((cards == null) || (xyBounds == null) || (xyBounds == Vector2.zero)) return;

        updateSeq?.Kill();
        updateSeq = DOTween.Sequence();

        int count = cards.Count;
        if (count < 1) return;

        int index = count - 1;
        float availableWidth = xyBounds.x;
        float actualSpacing = spacing;
        float requiredWidth = index * spacing;

        // Compress width in case the space is less than necessary 
        if (requiredWidth > availableWidth && count > 1)
        {
            actualSpacing = availableWidth / index;
        }

        float totalWidth = index * actualSpacing;
        float startOffset = -totalWidth / 2f;

        Vector3 basePosition = transform.position;

        for (int i = 0; i < count; i++)
        {
            CardVisual card = cards[i];
            if (card == null)
            {
                cards.Remove(card);
                i--;
                continue;
            }

            // Horizontal position
            float xOffset = startOffset + i * actualSpacing;

            // Normalized (-1 to 1) position in relation to the center
            float xNormalized = (totalWidth > 0f) ? (xOffset / (totalWidth / 2f)) : 0f;

            // Vertical position following: y = (1 - x²) * H
            float yOffset = (1 - (xNormalized * xNormalized)) * xyBounds.y;

            // Rotation angle. Left -1, center 0, right 1
            float angle = -xNormalized * arcDeggres;

            Vector3 targetPosition = basePosition +
            (transform.right * xOffset) +
            (transform.up * yOffset) +
            (i * kIndexOffsetZ * transform.forward);

            Quaternion targetRotation = transform.rotation * Quaternion.Euler(0f, 0f, angle);

            if (Application.isPlaying)
            {
                updateSeq.Join(card.transform.DOMove(targetPosition, updateDuration));
                updateSeq.Join(card.transform.DORotateQuaternion(targetRotation, updateDuration));
            }
            else card.transform.SetPositionAndRotation(targetPosition, targetRotation);
        }

        if (Application.isPlaying)
        {
            updateSeq.SetEase(Ease.OutQuad);
            updateSeq.Play();
        }
    }

    public void AddCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && !cards.Contains(card))
        {
            cards.Add(card);
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    public void RemoveCard(CardVisual card, bool ignoreUpdate = false)
    {
        if (card != null && cards.Remove(card))
        {
            if (ignoreUpdate) return;
            UpdateLayout();
        }
    }

    private void OnCardEnter() {}
    private void OnCardExit() {} 

    private void OnDrawGizmosSelected()
    {
        Matrix4x4 originalMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new(xyBounds.x, xyBounds.y, 0.05f));
        
        Gizmos.matrix = originalMatrix;
    }
}
