using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeTailGenerator : MonoBehaviour
{
    [SerializeField] private GameObject _tailcubePrefab;

    public void CreateCube(Vector3 spawnPosition)
    {
        Instantiate(_tailcubePrefab, spawnPosition, Quaternion.identity);
    }
}
