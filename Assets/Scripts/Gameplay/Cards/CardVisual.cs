using TMPro;
using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer artHolder;
    [SerializeField] private TMP_Text nickname, description;

    public void Setup(CardRuntime info)
    {
        if (info == null) return;

        artHolder.sprite = info.Artwork;
        nickname.SetText(info.Nickname);
        description.SetText(info.Description);
    }
}
