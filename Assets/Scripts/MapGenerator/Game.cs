using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Transition _transition;

    [Header("Transition Colors")]
    [SerializeField] private Material _goodMaterial;
    [SerializeField] private Material _badMaterial;

    public event Action Started;
    public event Action Paused;
    public event Action Continued;
    public event Action Ended;
    public event Action Restarted;
    public event Action Over;

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    public void ContinueGame()
    {
        StartCoroutine(ContinueGameRoutine());
    }

    public void GameOver(string text)
    {
        StartCoroutine(GameOverRoutine(text));
    }

    public void RestartGame()
    {
        StartCoroutine(GameRestartRoutine());
    }

    public void EndGame()
    {
        StartCoroutine(GameEndRoutine());
    }

    public void StopGameTime()
    {
        Time.timeScale = 0f;
    }

    public void ContinueGameTime()
    {
        Time.timeScale = 1f;
    }

    private IEnumerator StartGameRoutine()
    {
        yield return StartCoroutine(_transition.StartTransition(_goodMaterial.color));
        Started?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Debug.Log("Игра началась!");
    }

    private IEnumerator ContinueGameRoutine()
    {
        Continued?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Debug.Log("Игра продолжается.");
    }

    private IEnumerator GameEndRoutine()
    {
        yield return StartCoroutine(_transition.StartBackTransition(_goodMaterial.color));
        Ended?.Invoke();
        Debug.Log("Игра успешно окончена.");
    }

    private IEnumerator GameOverRoutine(string text)
    {
        yield return StartCoroutine(_transition.StartBackTransition(_badMaterial.color));
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
