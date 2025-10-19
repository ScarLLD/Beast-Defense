using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Transition _transition;

    [Header("Transition Colors")]
    [SerializeField] private Material _goodMaterial;
    [SerializeField] private Material _badMaterial;

    [Header("Other settings")]
    [SerializeField] private DeathModule _deathModule;

    private Coroutine _currentCoroutine;

    public event Action Started;
    public event Action Continued;
    public event Action Over;
    public event Action Completed;
    public event Action Restarted;
    public event Action Leaved;
    public event Action Transited;

    public bool HasCompleted { get; private set; }
    public bool HasStarted { get; private set; }
    public bool IsPause { get; private set; } = false;
    public bool IsPlaying { get; private set; } = false;

    private void OnEnable()
    {
        _deathModule.SnakeDie += CompleteGame;
    }

    private void OnDisable()
    {
        _deathModule.SnakeDie -= CompleteGame;
    }

    public void StartGame()
    {
        StartRoutine(StartGameRoutine());
    }

    public void ContinueGame()
    {
        StartRoutine(ContinueGameRoutine());
    }

    public void GameOver()
    {
        StartRoutine(GameOverRoutine());
    }

    public void RestartGame()
    {
        StartRoutine(GameRestartRoutine());
    }

    public void CompleteGame()
    {
        StartRoutine(GameCompleteRoutine());
    }

    public void LeaveGame()
    {
        StartRoutine(GameLeaveRoutine());
    }

    public void FastLeaveGame()
    {
        StartRoutine(FastGameLeaveRoutine());
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
        _transition.SetText("������");
        yield return StartCoroutine(_transition.StartTransition(_goodMaterial.color));
        Started?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        HasStarted = true;
        HasCompleted = false;
        IsPlaying = true;
        Debug.Log("���� ��������!");
        ClearRoutine();
    }

    private IEnumerator ContinueGameRoutine()
    {
        Continued?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Transited?.Invoke();
        HasCompleted = false;
        IsPlaying = true;
        Debug.Log("���� ������������.");
        ClearRoutine();
    }

    private IEnumerator GameCompleteRoutine()
    {
        Completed?.Invoke();
        _transition.SetText(string.Empty);
        yield return StartCoroutine(_transition.StartBackTransition(_goodMaterial.color));
        HasCompleted = true;
        IsPlaying = false;
        Debug.Log("���� ������� ��������.");
        ClearRoutine();
    }

    private IEnumerator GameLeaveRoutine()
    {
        IsPlaying = false;
        Leaved?.Invoke();
        yield return StartCoroutine(_transition.ContinueBackTransition());
        Transited?.Invoke();
        Debug.Log("���� ��������!");
        ClearRoutine();
    }

    private IEnumerator FastGameLeaveRoutine()
    {
        _transition.SetText("�����");
        yield return StartCoroutine(_transition.StartBackTransition(_badMaterial.color));
        IsPlaying = false;
        HasCompleted = false;
        Leaved?.Invoke();
        yield return StartCoroutine(_transition.ContinueBackTransition());
        Debug.Log("���� ��������!");
        ClearRoutine();
    }

    private IEnumerator GameOverRoutine()
    {
        IsPlaying = false;
        HasCompleted = false;
        Over?.Invoke();
        _transition.SetText(string.Empty);
        yield return StartCoroutine(_transition.StartBackTransition(_badMaterial.color));
        Debug.Log($"���� ���������!");
        ClearRoutine();
    }

    private IEnumerator GameRestartRoutine()
    {
        Restarted.Invoke();
        yield return StartCoroutine(_transition.ContinueTransition());
        Transited?.Invoke();
        IsPlaying = true;
        Debug.Log("���� ������������!");
        ClearRoutine();
    }

    private void StartRoutine(IEnumerator routine)
    {
        _currentCoroutine ??= StartCoroutine(routine);
    }

    private void ClearRoutine()
    {
        if (_currentCoroutine != null)
            _currentCoroutine = null;
    }
}
