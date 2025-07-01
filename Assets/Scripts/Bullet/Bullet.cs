using System;
using UnityEngine;

[RequireComponent(typeof(BulletMover))]
public class Bullet : MonoBehaviour
{
    public bool IsAvailable { get; private set; } = true;

    private BulletMover _mover;

    private void Awake()
    {
        _mover = GetComponent<BulletMover>();
    }

    private void OnEnable()
    {
        _mover.Arrived += StopMove;
    }

    private void OnDisable()
    {
        _mover.Arrived -= StopMove;
    }

    public void Init(Cube cube)
    {
        if (cube == null)
            throw new ArgumentNullException(nameof(cube), $"cube не может быть null.");

        _mover.Init(cube);
    }

    private void StopMove()
    {
        _mover.StopMove();
        gameObject.SetActive(false);
    }
}
