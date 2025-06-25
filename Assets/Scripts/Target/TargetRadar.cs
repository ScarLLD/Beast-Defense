using System;
using System.Collections;
using UnityEngine;

public class TargetRadar : MonoBehaviour
{
    private bool _isWork;
    private Coroutine _moveCoroutine;
    
    public void StartScanning()
    {
        _isWork = true;
        _moveCoroutine = StartCoroutine(ScanRoutine());
    }

    public void EndScan()
    {
        _isWork = false;

        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (_isWork)
        {


            yield return new WaitForSeconds(0.2f);
        }
    }
}