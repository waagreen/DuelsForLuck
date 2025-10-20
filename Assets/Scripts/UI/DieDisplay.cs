using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DieDisplay : MonoBehaviour
{
    [SerializeField] private Image faceHolder;
    [SerializeField] private List<Sprite> faces;

    private Tween showTween, hideTween;
    private Sequence disableSeq;
    private Color defaultColor;

    public void VisualSetup(Color defaultColor)
    {
        transform.localScale = Vector3.zero;

        this.defaultColor = defaultColor;
        faceHolder.color = defaultColor;

        gameObject.SetActive(false);
    }

    public Tween Disable(float duration)
    {
        disableSeq?.Kill();
        disableSeq = DOTween.Sequence();

        disableSeq.Append(transform.DOScale(Vector3.one * 0.8f, duration).SetEase(Ease.InBack));
        disableSeq.Join(faceHolder.DOColor(Color.grey, duration).SetEase(Ease.OutCubic));
        return disableSeq;
    }

    public Tween ShowFace(int value, float duration)
    {
        if (faces == null || faces.Count < 1) return null;

        faceHolder.sprite = faces[value];
        gameObject.SetActive(true);

        showTween?.Kill();
        showTween = transform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
        return showTween;
    }

    public Tween Hide(float duration)
    {
        hideTween?.Kill();
        hideTween = transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack);
        return hideTween;
    }

    public void Reset()
    {
        gameObject.SetActive(false);
        faceHolder.color = defaultColor;
    }
}
