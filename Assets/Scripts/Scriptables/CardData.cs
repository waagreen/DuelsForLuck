using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/CardData")]
public class CardData : ScriptableObject
{
    public string id;
    public string nickname;
    public string description;
    public Sprite artwork;
    public List<EffectData> effects;
}
