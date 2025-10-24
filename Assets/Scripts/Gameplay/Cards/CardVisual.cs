using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private Image artHolder;
    [SerializeField] private TMP_Text nickname, description;

    private CardRuntime info;

    public void Setup(CardRuntime info)
    {
        if (info == null) return;

        this.info = info;

        artHolder.sprite = info.Artwork;
        nickname.SetText(info.Nickname);
        description.SetText(info.Description);
    }
}
