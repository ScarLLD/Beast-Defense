using System.Collections;
using UnityEngine;

public class TargetRadar : MonoBehaviour
{
    [SerializeField] Shooter _shooter;

    private TargetStorage _targetStorage;
    private Coroutine _moveCoroutine;

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

        while (_shooter.BulletCount > 0)
        {
            if (bulletsPerSegment > 0 && _targetStorage.TryGetTarget(color, out SnakeSegment snakeSegment))
            {
                snakeSegment.SetIsTarget(true);
                _shooter.AddTarget(snakeSegment);
                bulletsPerSegment--;
            }

            yield return new WaitForSeconds(0.2f);
        }

        EndScan();
    }
}