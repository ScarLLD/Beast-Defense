using System;
using System.Collections;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Coroutine _moveCoroutine;

    public void Init(Transform targetTransform)
    {
        _moveCoroutine = StartCoroutine(MoveRoutine(targetTransform));
    }

    public void StopMoving()
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

            yield return null;
        }
    }
}
