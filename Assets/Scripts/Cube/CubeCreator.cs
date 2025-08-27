using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeCreator : MonoBehaviour
{
    [SerializeField] private GridStorage _gridStorage;
    [SerializeField] private PlayerCubeSpawner _cubeSpawner;

    [SerializeField]
    private List<Material> _ñolors = new();

    [SerializeField]
    private List<int> _counts = new();

    public bool TryCreate()
    {
        if (_gridStorage.GridCount > 0)
        {
            int gridCount = _gridStorage.GridCount;

            if (gridCount == 0)
                return false;

            for (int i = 0; i < gridCount; i++)
            {
                int count = _counts[Random.Range(0, _counts.Count)];
                Material material = _ñolors[Random.Range(0, _ñolors.Count)];

                if (_gridStorage.TryGet(i, out GridCell gridCell))
                {
                    _cubeSpawner.Spawn(material, count, gridCell);
                }
            }

            return true;
        }

        return false;
    }
}
