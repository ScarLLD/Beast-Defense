using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Transition _transition;

    [Header("Transition Colors")]
    [SerializeField] private Material _goodMaterial;
    [SerializeField] private Material _badMaterial;

    private Coroutine _currentCoroutine;

    public event Action Started;
    public event Action Continued;
    public event Action Ended;
    public event Action Restarted;
    public event Action Over;
    public event Action Leaved;

    public bool IsPause = false;

    public void StartGame()
    {
        StartRoutine(StartGameRoutine());
    }

    public void ContinueGame()
    {
        StartRoutine(ContinueGameRoutine());
    }

    public void GameOver(string text)
    {
        StartRoutine(GameOverRoutine(text));
    }

    public void RestartGame()
    {
        StartRoutine(GameRestartRoutine());
    }

    public void EndGame()
    {
        StartRoutine(GameEndRoutine());
    }

    public void LeaveGame()
    {
        StartRoutine(GameLeaveRoutine());
    }

    public void StopGameTime()
    {
        Time.timeScale = 0f;
        IsPause = true;
    }

    public void ContinueGameTime()
    {
        Time.timeScale = 1f;
        IsPause = false;
    }

    private IEnumerator StartGameRoutine()
    {
        yield return StartCoroutine(_transition.StartTransition(_goodMaterial.color));
        Started?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Debug.Log("Игра началась!"); 
        ClearRoutine();
    }

    private IEnumerator ContinueGameRoutine()
    {
        Continued?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Debug.Log("Игра продолжается.");
        ClearRoutine();
    }

    private IEnumerator GameEndRoutine()
    {
        yield return StartCoroutine(_transition.StartBackTransition(_goodMaterial.color));
        Ended?.Invoke();
        Debug.Log("Игра успешно окончена.");
        ClearRoutine();
    }

    private IEnumerator GameLeaveRoutine()
    {
        yield return StartCoroutine(_transition.StartBackTransition(_badMaterial.color));
        Leaved?.Invoke();
        yield return StartCoroutine(_transition.ContinueBackTransition());
        Debug.Log("Игра покинута");
        ClearRoutine();
    }

    private IEnumerator GameOverRoutine(string text)
    {
        yield return StartCoroutine(_transition.StartBackTransition(_badMaterial.color));
        Over?.Invoke();
        Debug.Log($"Игра окончена! {text}");
        ClearRoutine();
    }

    private IEnumerator GameRestartRoutine()
    {
        yield return StartCoroutine(_transition.ContinueBackTransition());
        Restarted.Invoke();
        Debug.Log("Игра перезапущена!");
        ClearRoutine();
    }

    private void StartRoutine(IEnumerator routine)
    {
        if (_currentCoroutine == null)
            _currentCoroutine = StartCoroutine(routine);
    }

    private void ClearRoutine()
    {
        if (_currentCoroutine != null)
            _currentCoroutine = null;
    }
}
