using UnityEngine;

[CreateAssetMenu(fileName = "CardInfo", menuName = "Scriptable Objects/CardInfo")]
public class CardData : ScriptableObject
{
    public string id;
    public string nickname;
    public string description;
    public Sprite artwork;
    
    // TODO: Store card effect in some robust way
}
