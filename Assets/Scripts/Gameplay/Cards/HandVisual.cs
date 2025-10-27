using UnityEngine;

public class HandVisual : MonoBehaviour
{
    [SerializeField] private HorizontalCardLayout layout;
    [SerializeField] private int ownerStartingOrder;

    public HorizontalCardLayout Layout => layout;
}
