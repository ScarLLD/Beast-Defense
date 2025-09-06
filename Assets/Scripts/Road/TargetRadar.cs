using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Shooter))]
public class TargetRadar : MonoBehaviour
{
    private Shooter _shooter;
    private TargetStorage _targetStorage;
    private Coroutine _moveCoroutine;

    private void Awake()
    {
        _shooter = GetComponent<Shooter>();
    }

    public void Init(TargetStorage targetStorage)
    {
        _targetStorage = targetStorage;
    }

    public void StartScanning(Color color)
    {
        _moveCoroutine ??= StartCoroutine(ScanRoutine(color));
    }

    private void EndScan()
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }
    }

    private IEnumerator ScanRoutine(Color color)
    {
        int bulletsPerSegment = _shooter.BulletCount / 4;

        _shooter.SetInitialRotation();

        while (_shooter.BulletCount > 0)
        {
            if (bulletsPerSegment > 0 && _targetStorage.TryGetTarget(color, out SnakeSegment snakeSegment))
            {
                _shooter.AddTarget(snakeSegment);
                bulletsPerSegment--;
            }

            yield return new WaitForSeconds(0.2f);
        }

        EndScan();
    }
}