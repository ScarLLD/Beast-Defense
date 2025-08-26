using System;
using System.Collections;
using UnityEngine;

public class SnakeMouth : MonoBehaviour
{
    private readonly float _eatThreshold = 0.3f;
    private Coroutine _coroutine;
    private Beast _beast;

    public void Init(Beast beast)
    {
        if (beast == null)
            throw new ArgumentException(nameof(beast), "beast не может быть null.");

        _beast = beast;

        StartEatRoutine();
    }

    private void StartEatRoutine()
    {
        _coroutine ??= StartCoroutine(Eat());
    }

    private IEnumerator Eat()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_beast != null && (transform.position - _beast.transform.position).magnitude < _eatThreshold)
            {
                _beast.Destroy();
            }

            yield return null;
        }
    }
}
