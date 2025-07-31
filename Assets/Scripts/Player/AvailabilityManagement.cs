using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailabilityManagement : MonoBehaviour
{
    [SerializeField] private CubeStorage _cubeStorage;
    [SerializeField] private CubeCreator _cubeCreator;
    [SerializeField] private GridCreator _gridCreator;

    private List<PlayerCube>[,] _stacks;

    private void OnEnable()
    {
        _cubeCreator.Created += OnCubesCreated;
    }

    private void OnDisable()
    {
        _cubeCreator.Created -= OnCubesCreated;
    }

    private void OnCubesCreated()
    {
        _stacks = new List<PlayerCube>[_gridCreator.Rows, _gridCreator.Columns];

        for (int i = 0; i < _stacks.GetLength(0); i++)
        {
            for (int j = 0; j < _stacks.GetLength(1); j++)
            {
                _stacks[i, j] = new List<PlayerCube>();

                int index = i * _gridCreator.Columns + j;

                if (index < _cubeStorage.Stacks.Count && _cubeStorage.Stacks[index] is PlayerCube cube)
                {
                    _stacks[i, j].Add(cube);
                }
            }
        }

        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        for (int i = 0; i < _stacks.GetLength(0); i++)
        {
            for (int j = 0; j < _stacks.GetLength(1); j++)
            {
                foreach (var cube in _stacks[i, j])
                {
                    if (cube.IsStatic)
                    {
                        bool isTopRow = i == _stacks.GetLength(0) - 1;
                        bool isLeftEdge = j == 0;
                        bool isRightEdge = j == _stacks.GetLength(1) - 1;
                        bool isBottomEdge = i == 0;

                        if (isTopRow)
                        {
                            cube.ChangeAvailableStatus(true);
                            continue;
                        }

                        bool haveStaticLeft = isLeftEdge == false && _stacks[i, j - 1].Any(c => c.IsStatic);
                        bool haveStaticRight = isRightEdge == false && _stacks[i, j + 1].Any(c => c.IsStatic);
                        bool haveStaticBottom = isBottomEdge == false && _stacks[i - 1, j].Any(c => c.IsStatic);
                        bool haveStaticTop = isTopRow == false && _stacks[i + 1, j].Any(c => c.IsStatic);

                        bool isAvailable = false;

                        if (isBottomEdge)
                        {
                            if (isLeftEdge && (haveStaticTop == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isRightEdge && (haveStaticTop == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isLeftEdge == false && isRightEdge == false && (haveStaticTop == false || haveStaticLeft == false || haveStaticRight == false))
                                isAvailable = true;
                        }
                        else if (isLeftEdge)
                        {
                            if (isTopRow && (haveStaticBottom == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isBottomEdge && (haveStaticTop == false || haveStaticRight == false))
                                isAvailable = true;
                            else if (isBottomEdge == false && isTopRow == false && (haveStaticTop == false || haveStaticRight == false || haveStaticBottom == false))
                                isAvailable = true;
                        }
                        else if (isRightEdge)
                        {
                            if (isTopRow && (haveStaticBottom == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isBottomEdge && (haveStaticTop == false || haveStaticLeft == false))
                                isAvailable = true;
                            else if (isBottomEdge == false && isTopRow == false && (haveStaticTop == false || haveStaticLeft == false || haveStaticBottom == false))
                                isAvailable = true;
                        }
                        else if (!isLeftEdge && !isRightEdge && !isBottomEdge && (!haveStaticLeft || !haveStaticRight || !haveStaticBottom || !haveStaticTop))
                        {
                            isAvailable = true;
                        }

                        cube.ChangeAvailableStatus(isAvailable);
                    }
                }
            }
        }
    }
}