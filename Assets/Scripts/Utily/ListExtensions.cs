using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// Contains useful methods for list manipulation.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Shuffles a list using the Fisher-Yates algorithm.
    /// </summary>
    public static void Shuffle<T>(this List<T> list)
    {
        // No elements to shuffle.
        if (list == null || list.Count <= 1)
        {
            return;
        }

        // Fisher-Yates
        int n = list.Count;
        while (n > 1)
        {
            n--;
            
            // Chooses a random index 'k' between 0 and n (inclusive).
            int k = Random.Range(0, n + 1);

            // Swap 'k' for 'n' with tuples
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}