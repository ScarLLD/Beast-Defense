using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastMover : MonoBehaviour
{
    private Coroutine _coroutine;

    public Vector3 CurrentRoadPoint { get; private set; }

    public void StartMove()
    {
        _coroutine ??= StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        bool isWork = true;

        while (isWork)
        {
            //Move


            yield return null;
        }
    }
}
