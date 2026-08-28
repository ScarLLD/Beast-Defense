using System.Collections;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Road
{
    [RequireComponent(typeof(Shooter))]
    public class TargetRadar : MonoBehaviour
    {
        private const float TARGETING_DELAY = 0.1f;

        private Coroutine _scanCoroutine;
        private Shooter _shooter;
        private WaitForSeconds _targetSleep;
        private TargetStorage _targetStorage;
        private int _bulletPerTarget;

        private void Awake()
        {
            _shooter = GetComponent<Shooter>();

            _targetSleep = new WaitForSeconds(TARGETING_DELAY);
        }

        public void Init(TargetStorage targetStorage, int bulletPerTarget)
        {
            _targetStorage = targetStorage;
            _bulletPerTarget = bulletPerTarget;
        }

        public void StartScanning(Color color)
        {
            _scanCoroutine ??= StartCoroutine(ScanRoutine(color));
        }

        public void TurnOff()
        {
            if (_scanCoroutine == null) return;

            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }

        private IEnumerator ScanRoutine(Color color)
        {
            var bulletsPerSegment = _shooter.BulletCount / _bulletPerTarget;

            _shooter.SetInitialRotation();

            while (_shooter.BulletCount > 0)
            {
                if (bulletsPerSegment > 0 && _targetStorage.TryGetTarget(color, out var snakeSegment))
                {
                    _shooter.AddTarget(snakeSegment);
                    bulletsPerSegment--;
                }

                yield return _targetSleep;
            }

            _scanCoroutine = null;
        }
    }
}