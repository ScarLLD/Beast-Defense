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
        _timer.StopTimer(true);
        StartCoroutine(KillSnakeRoutine(gameObject));
    }

    public void KillBeast(Transform gameObject)
    {
        _timer.StopTimer(false);
        StartCoroutine(KillBeastRoutine(gameObject));
    }

    public IEnumerator DeathRoutine(Transform gameObject, Color color)
    {
        yield return StartCoroutine(_animator.DeathRoutine(gameObject, color));
    }

    private IEnumerator KillSnakeRoutine(Transform gameObject)
    {
        yield return StartCoroutine(DeathRoutine(gameObject, Color.red));
        SnakeDie?.Invoke();
    }

    private IEnumerator KillBeastRoutine(Transform gameObject)
    {
        yield return StartCoroutine(DeathRoutine(gameObject, Color.white));
        BeastDie?.Invoke();
    }
}
