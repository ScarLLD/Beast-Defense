using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BeastMover), typeof(BeastRotator))]
public class Beast : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dieParticle;

    private List<Vector3> _road;
    private BeastRotator _beastRotator;

    public BeastMover Mover { get; private set; }

    private void Awake()
    {
        _beastRotator = GetComponent<BeastRotator>();
        Mover = GetComponent<BeastMover>();
    }

    public void Init(List<Vector3> road, Snake snake)
    {
        if (snake == null)
            throw new ArgumentException("road не может быть null.", nameof(snake));

        if (road == null || road.Count == 0)
            throw new ArgumentOutOfRangeException("road не может быть null или быть пустым.", nameof(road));

        _road = road;

        Mover.Init(snake);
        Mover.SetRoadTarget(_road);
        Mover.StartMoveRoutine();

        _beastRotator.Init(snake);
        _beastRotator.StartRotateRoutine();
    }

    public float GetNormalizedDistance()
    {
        return (float)_road.IndexOf(Mover.TargetPoint) / (float)_road.Count;
    }

    public bool TryGetNextRoadPosition(out Vector3 nextPosition)
    {
        nextPosition = Vector3.zero;

        if (_road != null && _road.Contains(Mover.TargetPoint))
        {
            int currentIndex = _road.IndexOf(Mover.TargetPoint);

            if (currentIndex + 1 < _road.Count)
            {
                nextPosition = _road[currentIndex + 1];
            }
        }

        return nextPosition != Vector3.zero;
    }

    public void Destroy()
    {
        var particle = Instantiate(_dieParticle);
        particle.transform.position = transform.position;
        Destroy(this.gameObject);
    }
}
