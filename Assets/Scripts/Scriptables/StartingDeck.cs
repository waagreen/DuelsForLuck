using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DeckEntry
{
    public CardData data;
    public int amount;
}

[CreateAssetMenu(fileName = "StartingDeck", menuName = "Cards/StartingDeck")]
public class StartingDeck : ScriptableObject
{
    public List<DeckEntry> cards;
}
