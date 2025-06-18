using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesCounterHolder : MonoBehaviour
{
    [SerializeField]
    private IReadOnlyList<int> availableCount = new List<int>()
    {
        8,
        16,
        32
    };

    public int GetRandomCount() => availableCount[Random.Range(0, availableCount.Count)];
}
