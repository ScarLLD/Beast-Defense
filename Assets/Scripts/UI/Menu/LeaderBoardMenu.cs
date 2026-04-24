using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardMenu : Window
{
    [SerializeField] private Transition _transition;
    [SerializeField] private Material _leaderBoardMaterial;
    [SerializeField] private float _transitionDuration = 0.4f;
    [SerializeField] private Button _exitButton;

    public event Action Opened;
    public event Action Closed;

    private void Awake()
    {
        DisableMenu();
    }

    private void OnEnable()
    {
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    public void Open()
    {
        if (_transition.IsTransiting == false)
            StartCoroutine(OpenLeaderBoardRoutine());
    }

    private IEnumerator OpenLeaderBoardRoutine()
    {
        _transition.SetText("Загрузка");
        yield return StartCoroutine(_transition.StartTransitionRoutine(_leaderBoardMaterial.color, _transitionDuration));
        EnableMenu();
        Opened?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine(_transitionDuration));
    }

    private void OnExitButtonClick()
    {
        if (_transition.IsTransiting == false)
            StartCoroutine(CloseLeaderBoardRoutine());
    }

    private IEnumerator CloseLeaderBoardRoutine()
    {
        if (_transition.IsTransiting == false)
        {
            _transition.SetText("Выход");
            yield return StartCoroutine(_transition.StartBackTransitionRoutine(_leaderBoardMaterial.color, _transitionDuration));
            Closed?.Invoke();
            DisableMenu();
            yield return StartCoroutine(_transition.ContinueBackTransitionRoutine(_transitionDuration));
        }
    }
}
