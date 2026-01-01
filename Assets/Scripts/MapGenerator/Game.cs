using System;
using System.Collections;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Transition _transition;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private GameOverMenu _gameOverMenu;
    [SerializeField] private VictoryMenu _victoryMenu;
    [SerializeField] private MainMenu _mainMenu;

    [Header("Transition Colors")]
    [SerializeField] private Material _goodMaterial;
    [SerializeField] private Material _badMaterial;

    [Header("Other settings")]
    [SerializeField] private DeathModule _deathModule;
    [SerializeField] private GameHeart _gameHeart;

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

    private void OnApplicationQuit()
    {
        if (IsPlaying)
            _gameHeart.TryDecreaseCount();
    }

    public void StartGame()
    {
        if (_transition.IsTransiting == false && _gameHeart.IsPossibleDecrease == true)
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
        _transition.SetText("Запуск");
        yield return StartCoroutine(_transition.StartTransitionRoutine(_goodMaterial.color));
        Started?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine());
        HasStarted = true;
        HasCompleted = false;
        IsPlaying = true;
        Debug.Log("Игра началась!");
        ClearRoutine();
    }

    private IEnumerator ContinueGameRoutine()
    {
        Continued?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine());
        Transited?.Invoke();
        HasCompleted = false;
        IsPlaying = true;
        Debug.Log("Игра продолжается.");
        ClearRoutine();
    }

    private IEnumerator GameCompleteRoutine()
    {
        IsPlaying = false;
        HasCompleted = true;
        Completed?.Invoke();
        _transition.SetText(string.Empty);
        yield return StartCoroutine(_transition.StartBackTransitionRoutine(_goodMaterial.color));
        _gameHeart.transform.SetParent(_canvasTransform.transform);
        _gameHeart.gameObject.SetActive(true);
        Debug.Log("Игра успешно окончена.");
        ClearRoutine();
    }

    private IEnumerator GameLeaveRoutine()
    {
        IsPlaying = false;
        Leaved?.Invoke();
        _gameHeart.transform.SetParent(_canvasTransform.transform);
        yield return StartCoroutine(_transition.ContinueBackTransitionRoutine());
        _gameHeart.transform.SetParent(_mainMenu.transform);
        Transited?.Invoke();
        Debug.Log("Игра покинута!");
        ClearRoutine();
    }

    private IEnumerator FastGameLeaveRoutine()
    {
        _transition.SetText("Выход");
        yield return StartCoroutine(_transition.StartBackTransitionRoutine(_badMaterial.color));
        IsPlaying = false;
        HasCompleted = false;
        Leaved?.Invoke();
        _gameHeart.transform.SetParent(_mainMenu.transform);
        _gameHeart.gameObject.SetActive(true);
        yield return StartCoroutine(_transition.ContinueBackTransitionRoutine());
        yield return StartCoroutine(_gameHeart.UseHeartRoutine());
        Debug.Log("Игра покинута!");
        ClearRoutine();
    }

    private IEnumerator GameOverRoutine()
    {
        IsPlaying = false;
        HasCompleted = false;
        Over?.Invoke();
        _transition.SetText(string.Empty);
        yield return StartCoroutine(_transition.StartBackTransitionRoutine(_badMaterial.color));
        _gameHeart.transform.SetParent(_gameOverMenu.transform);
        _gameHeart.gameObject.SetActive(true);
        yield return StartCoroutine(_gameHeart.UseHeartRoutine());
        Debug.Log($"Игра проиграна!");
        ClearRoutine();
    }

    private IEnumerator GameRestartRoutine()
    {
        Restarted.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine());
        _gameHeart.gameObject.SetActive(false);
        Transited?.Invoke();
        IsPlaying = true;
        Debug.Log("Игра перезапущена!");
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
