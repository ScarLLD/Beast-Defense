using System;
using System.Collections;
using UnityEngine;

public class DeathModule : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameTimer _timer;
    [SerializeField] private DeathAnimator _animator;

    public event Action BeastDie;
    public event Action SnakeDie;

    public void KillSnake(Transform gameObject)
    {
        _timer.StopTimer();
        StartCoroutine(KillSnakeRoutine(gameObject));
    }

    public void KillBeast(Transform gameObject)
    {
        _timer.StopTimer();
        StartCoroutine(KillBeastRoutine(gameObject));
    }

    public IEnumerator DeathRoutine(Transform gameObject)
    {
        yield return StartCoroutine(_animator.DeathRoutine(gameObject));


    }

    private IEnumerator KillSnakeRoutine(Transform gameObject)
    {
        _animator.SetParticleColor(Color.red);
        yield return StartCoroutine(DeathRoutine(gameObject));
        SnakeDie?.Invoke();
    }

    private IEnumerator KillBeastRoutine(Transform gameObject)
    {
        _animator.SetParticleColor(Color.white);
        yield return StartCoroutine(DeathRoutine(gameObject));
        BeastDie?.Invoke();
    }
}
