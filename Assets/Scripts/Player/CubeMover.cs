using System;
using System.Collections;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    private float _speed;
    private float _maxDistance;
    private Coroutine _moveCoroutine;

    public event Action Arrived;

    public void Init(float speed, float maxDistance)
    {
        _speed = speed;
        _maxDistance = maxDistance;
    }

    public void MoveTarget(Transform targetTransform)
    {
        _moveCoroutine = StartCoroutine(MoveRoutine(targetTransform));
    }

    private void OnDisable()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator MoveRoutine(Transform target)
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = target.position - transform.position;
            transform.Translate(_speed * Time.deltaTime * direction.normalized);

            if ((target.position - transform.position).magnitude < _maxDistance)
            {
                Arrived?.Invoke();
                isWork = false;
            }

            yield return null;
        }
    }
}
