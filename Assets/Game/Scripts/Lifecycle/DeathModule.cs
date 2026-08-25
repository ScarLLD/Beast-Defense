using Game.Scripts.Effects;
using Game.Scripts.Options;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.LifeCycle
{
    public class DeathModule : MonoBehaviour
    {
        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private DeathAnimator _animator;
        [SerializeField] private AudioPlayer _audioPlayer;

        public event Action BeastDied;
        public event Action SnakeDied;

        public void KillSnake(Transform killTarget)
        {
            _timer.StopTimer(true);
            StartCoroutine(KillSnakeRoutine(killTarget));
        }

        public void KillBeast(Transform killTarget)
        {
            _timer.StopTimer(false);
            StartCoroutine(KillBeastRoutine(killTarget));
        }

        private IEnumerator DeathRoutine(Transform killTarget, Color color)
        {
            yield return StartCoroutine(_animator.DeathRoutine(killTarget, color));
        }

        private IEnumerator KillSnakeRoutine(Transform killTarget)
        {
            _audioPlayer.PlaySnakeDieSound();
            yield return StartCoroutine(DeathRoutine(killTarget, Color.red));
            SnakeDied?.Invoke();
        }

        private IEnumerator KillBeastRoutine(Transform killTarget)
        {
            _audioPlayer.PlayBeastDieSound();
            yield return StartCoroutine(DeathRoutine(killTarget, Color.white));
            BeastDied?.Invoke();
        }
    }
}