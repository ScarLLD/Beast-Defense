using System;
using UnityEngine;

[RequireComponent(typeof(BulletMover))]
public class Bullet : MonoBehaviour
{
    private BulletMover _mover;

    public void Init(Transform targetTransform)
    {    
        if (targetTransform == null)
            throw new ArgumentNullException(nameof(targetTransform), $"targetTransform не может быть null.");

        _mover.Init(targetTransform);
    }

    private void Awake()
    {
        _mover = GetComponent<BulletMover>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out ITarget target))
        {            
            gameObject.SetActive(false);
        }
    }
}
