using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardMenu : Window
{
    [SerializeField] private Transition _transition;
    [SerializeField] private Material _leaderBoardMaterial;

    [SerializeField] private Button _exitButton;

    public event Action Opened;
    public event Action Closed;

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
        yield return StartCoroutine(_transition.StartTransitionRoutine(_leaderBoardMaterial.color));
        EnableMenu();
        Opened?.Invoke();
        yield return StartCoroutine(_transition.ContinueTransitionRoutine());
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
            yield return StartCoroutine(_transition.StartBackTransitionRoutine(_leaderBoardMaterial.color));
            Closed?.Invoke();
            DisableMenu();
            yield return StartCoroutine(_transition.ContinueBackTransitionRoutine());
        }
    }
}
