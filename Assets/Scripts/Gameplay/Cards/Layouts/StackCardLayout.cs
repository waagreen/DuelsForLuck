using DG.Tweening;
using UnityEngine;

public class StackCardLayout : CardLayout
{
    [Header("Stack Card Layout")]
    [SerializeField] private float heightOffset = 0.1f;
    
    public override void UpdateLayout()
    {
        base.UpdateLayout();

        int count = cards.Count;
        if (cards == null || count < 1) return;

        for (int i = 0; i < count; i++)
        {
            CardVisual card = cards[i];
            Vector3 targetPosition = transform.position + (heightOffset * i * transform.up);
            card.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

            if (Application.isPlaying)
            {
                updateSeq.Join(card.transform.DOMove(targetPosition, updateDuration));
            }
            else card.transform.position = targetPosition;
        }

        if (Application.isPlaying)
        {
            updateSeq.SetEase(Ease.OutQuad);
            updateSeq.Play();
        };
    }
}
