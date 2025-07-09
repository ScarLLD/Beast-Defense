using System;
using System.Collections;
using UnityEngine;


public class TargetRadar : MonoBehaviour
{
    [SerializeField] Shooter _shooter;

    private TargetStorage _targetStorage;
    private Coroutine _moveCoroutine;

    public event Action<SnakeSegment> Found;

    public void Init(TargetStorage targetStorage)
    {
        _targetStorage = targetStorage;
    }

    public void StartScanning(Color color)
    {
        _moveCoroutine = StartCoroutine(ScanRoutine(color));
    }

    public void EndScan()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator ScanRoutine(Color color)
    {
        while (_shooter.BulletCount > 0)
        {
            if (_targetStorage.TryGetTarget(color, out SnakeSegment snakeSegment))
                Found?.Invoke(snakeSegment);

            yield return new WaitForSeconds(0.2f);
        }
    }
}