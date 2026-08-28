using System;
using System.Collections;
using Game.Scripts.Effects;
using Game.Scripts.Options;
using UnityEngine;

namespace Game.Scripts.Lifecycle
{
    public class DeathModule : MonoBehaviour
    {
        [SerializeField] private GameTimer _timer;
        [SerializeField] private DeathAnimator _animator;
        [SerializeField] private AudioPlayer _audioPlayer;

        public event Action BeastDied;
        public event Action SnakeDied;

        public void KillSnake(Transform target)
        {
            _timer.StopTimer(true);
            StartCoroutine(KillSnakeRoutine(target));
        }

        public void KillBeast(Transform target)
        {
            _timer.StopTimer(false);
            StartCoroutine(KillBeastRoutine(target));
        }

        private IEnumerator KillSnakeRoutine(Transform target)
        {
            _audioPlayer.PlaySnakeDieSound();
            yield return StartCoroutine(DeathRoutine(target, Color.red));

            SnakeDied?.Invoke();
        }

        private IEnumerator KillBeastRoutine(Transform target)
        {
            _audioPlayer.PlayBeastDieSound();
            yield return StartCoroutine(DeathRoutine(target, Color.white));
            
            BeastDied?.Invoke();
        }

        private IEnumerator DeathRoutine(Transform target, Color color)
        {
            yield return StartCoroutine(_animator.DeathRoutine(target, color));
        }
    }
}