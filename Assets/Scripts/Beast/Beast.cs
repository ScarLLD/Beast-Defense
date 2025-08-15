using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BeastMover))]
public class Beast : MonoBehaviour
{
    [SerializeField] private float _speed;

    private List<Vector3> _road;

    private Vector3 _startPosition;

    public BeastMover Mover { get; private set; }
    public float Speed => _speed;

    private void Awake()
    {
        Mover = GetComponent<BeastMover>();
    }

    public void Init(List<Vector3> road)
    {
        if (road == null || road.Count == 0)
            throw new ArgumentOutOfRangeException("road не может быть null или быть пустым.", typeof(road));

        _road = road;

        Mover.SetRoadTarget(_road);
        Mover.StartMove();
    }

    public bool TryGetNextRoadPosition(out Vector3 nextPosition)
    {
        nextPosition = Vector3.zero;

        if (_road.Contains(Mover.LocalTargetPoint)
            && _road.Count >= _road.IndexOf(Mover.LocalTargetPoint) + 1)
        {
            nextPosition = _road[_road.IndexOf(Mover.LocalTargetPoint) + 1];
        }

        return nextPosition != Vector3.zero;
    }
}
