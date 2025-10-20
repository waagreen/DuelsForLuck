using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private List<MeshRenderer> meshes;

    private List<Color> cachedColors;
    private Sequence hurtSequence;

    private void Start()
    {
        cachedColors = new();

        foreach (MeshRenderer rend in meshes)
        {
            cachedColors.Add(rend.material.color);
        }
    }

    private void Restore()
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            MeshRenderer rend = meshes[i];
            Color original = cachedColors[i];
            rend.material.DOColor(original, 0.05f);
        }
    }

    public Tween Hurt()
    {
        hurtSequence?.Kill();
        hurtSequence = DOTween.Sequence();
        hurtSequence.Append(transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, vibrato: 3, elasticity: 0.3f));
        hurtSequence.JoinCallback(audioScr.Play);
        foreach (MeshRenderer rend in meshes)
        {
            hurtSequence.Join(rend.material.DOColor(Color.white, 0.05f).SetEase(Ease.Flash));
        }
        hurtSequence.OnComplete(Restore);
        return hurtSequence;
    }
}
