using System.Collections;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Coroutine _moveCoroutine;

    public void Init(Transform targetTransform)
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

            if (transform.position == target.transform.position)
                isWork = false;

            yield return null;
        }
    }
}
