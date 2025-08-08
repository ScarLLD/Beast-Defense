using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _arrivalThreshold = 0.1f;

    private Rigidbody _rigidbody;
    private Coroutine _moveCoroutine;

    public void InitTarget(Cube cube)
    {
        if (cube == null)
            throw new ArgumentNullException(nameof(cube), $"cube не может быть null.");

        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToTarget(cube));
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private IEnumerator MoveToTarget(Cube cube)
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = (cube.transform.position - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            if (Vector3.Distance(transform.position, cube.transform.position) < _arrivalThreshold)
            {
                _rigidbody.velocity = Vector3.zero;
                cube.Destroy();

                isWork = false;
            }

            yield return null;
        }

        _moveCoroutine = null;
        gameObject.SetActive(false);
    }
}