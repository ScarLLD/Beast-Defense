using System;
using System.Collections;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    private Rigidbody _rigidbody;
    private Coroutine _moveCoroutine;

    public event Action Arrived;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbody.useGravity = false;
        }
    }

    public void Init(Cube cube)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToTarget(cube));
    }

    public void StopMove()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
            _rigidbody.velocity = Vector3.zero;
        }
    }

    private IEnumerator MoveToTarget(Cube cube)
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = (cube.transform.position - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            if (Vector3.Distance(transform.position, cube.transform.position) < 1f)
            {
                Arrived?.Invoke();
                cube.Destroy();
                isWork = false;
                _rigidbody.velocity = Vector3.zero;
            }

            yield return null;
        }
    }
}