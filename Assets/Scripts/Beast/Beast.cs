using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(BeastMover), typeof(BeastRotator))]
public class Beast : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dieParticle;

    private BeastRotator _beastRotator;

    public BeastMover Mover { get; private set; }

    private void Awake()
    {
        _beastRotator = GetComponent<BeastRotator>();
        Mover = GetComponent<BeastMover>();
    }

    public void Init(Snake snake, SplineContainer splineContainer)
    {
        if (snake == null)
            throw new ArgumentException("Snake не может быть null.", nameof(snake));

        Mover.Init(snake);
        Mover.SetRoadTarget(splineContainer);
        Mover.StartMoveRoutine();

        _beastRotator.StartRotateRoutine();
    }

    public float GetNormalizedDistance()
    {
        return Mover.GetNormalizedDistance();
    }

    public bool TryGetNextRoadPosition(out Vector3 nextPosition)
    {
        nextPosition = Vector3.zero;
        // Этот метод теперь менее актуален при использовании сплайнов,
        // но оставлен для обратной совместимости
        return false;
    }

    public void Destroy()
    {
        var particle = Instantiate(_dieParticle);
        particle.transform.position = transform.position;
        Destroy(this.gameObject);
    }
}