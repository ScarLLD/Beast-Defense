using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Transition _transition;

    public event Action Started;
    public event Action Restarted;
    public event Action Over;

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public void GameOver(string text)
    {
        StartCoroutine(GameOverRoutine(text));
    }

    public void RestartGame()
    {
        StartCoroutine(GameRestartRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        yield return StartCoroutine(_transition.StartTransition());
        Started?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Debug.Log("Игра началась!");
    }

    private IEnumerator GameOverRoutine(string text)
    {
        yield return StartCoroutine(_transition.StartBackTransition());
        Over?.Invoke();
        Debug.Log($"Игра окончена! {text}");
    }

    private IEnumerator GameRestartRoutine()
    {
        yield return StartCoroutine(_transition.ContinueBackTransition());
        Restarted.Invoke();
        Debug.Log("Игра перезапущена!");
    }
}
