using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BeastMover))]
public class Beast : MonoBehaviour
{
    private List<Vector3> _road;

    public BeastMover Mover { get; private set; }

    private void Awake()
    {
        Mover = GetComponent<BeastMover>();
    }

    public void Init(List<Vector3> road, SnakeHead snakeHead)
    {
        if (road == null || road.Count == 0)
            throw new ArgumentOutOfRangeException("road не может быть null или быть пустым.", nameof(road));

        _road = road;

        Mover.Init(snakeHead);
        Mover.SetRoadTarget(_road);
        Mover.StartMove();
    }

    public bool TryGetNextRoadPosition(out Vector3 nextPosition)
    {
        nextPosition = Vector3.zero;

        if (_road != null && _road.Contains(Mover.LocalTargetPoint))
        {
            int currentIndex = _road.IndexOf(Mover.LocalTargetPoint);

            if (currentIndex + 1 < _road.Count)
            {
                nextPosition = _road[currentIndex + 1];
            }
        }

        return nextPosition != Vector3.zero;
    }

    public bool TryGetRoadIndex(Vector3 position, out int beastIndex)
    {
        beastIndex = 0;

        if (_road.Contains(position))
            beastIndex = _road.IndexOf(position);

        return beastIndex != 0;
    }
}
