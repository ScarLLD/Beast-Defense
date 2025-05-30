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

    public void Init(Transform targetTransform)
    {
        IsAvailable = false;
        if (targetTransform == null)
            throw new ArgumentNullException(nameof(targetTransform), $"targetTransform не может быть null.");

        _mover.Init(targetTransform);

    }       

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ITarget target))
        {
            _mover.StopMoving();
            gameObject.SetActive(false);
            IsAvailable = true;
        }
    }
}
