using System;
using System.Collections;
using UnityEngine;

public class BulletMover : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    public event Action Arrived;
    public Transform Target;

    private Coroutine _moveCoroutine;

    public void Init(Cube cube)
    {
        if (cube == null)
        {
            Target = cube.transform;
            Debug.LogError("Target is null!");
            return;
        }

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
        }
    }

    private IEnumerator MoveToTarget(Cube cube)
    {
        bool isWork = true;

        while (isWork)
        {
            Vector3 direction = (cube.transform.position - transform.position).normalized;
            transform.position += _speed * Time.deltaTime * direction;

            if (Vector3.Distance(transform.position, cube.transform.position) < 1f)
            {
                Arrived?.Invoke();
                cube.Destroy();
                isWork = false;
            }

            yield return null;
        }
    }
}