using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image artHolder;
    [SerializeField] private TMP_Text nickname, description;

    public void Setup(CardRuntime info)
    {
        if (info == null) return;

        artHolder.sprite = info.Artwork;
        nickname.SetText(info.Nickname);
        description.SetText(info.Description);
    }
}
