using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _targetTransform;

    internal void Init(Transform target)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target), $"target не может быть null.");

        _targetTransform = target;
    }

    void Update()
    {
        if (_targetTransform != null)
        {
            Vector3 direction = _targetTransform.position - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);
        }
    }
}
