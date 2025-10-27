using System.Collections.Generic;
using UnityEngine;

public class DeckVisual : MonoBehaviour
{
    [SerializeField] private StackCardLayout layout;
    [SerializeField] private int ownerStartingOrder;
    
    public StackCardLayout Layout => layout;
}
