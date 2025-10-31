using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActorModel : MonoBehaviour
{
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private List<AudioClip> clips;
    [SerializeField] private List<MeshRenderer> meshes;

    private List<Color> cachedColors;

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
            rend.material.DOColor(original, 0.01f);
        }
    }

    public Tween Colorize(Color color)
    {
        Sequence colorizeSeq = DOTween.Sequence();
        foreach (MeshRenderer rend in meshes)
        {
            colorizeSeq.Join(rend.material.DOColor(color, 0.05f).SetEase(Ease.OutExpo));
        }
        colorizeSeq.OnComplete(Restore);
        return colorizeSeq;
    }

    public Tween Hurt()
    {
        return transform.DOPunchScale(Vector3.one * 0.1f, 0.15f, vibrato: 3, elasticity: 0.3f);
    }

    public void PlaySound(CardEffectType effectType)
    {
        int index = (int)effectType;
        if (index < 0 || index > clips.Count) return;

        audioScr.clip = clips[index];
        audioScr.Play();
    }
}
