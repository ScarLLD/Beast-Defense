using System;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    [SerializeField] private BeastCollector _collector;

    public bool IsActive { get; private set; } = false;

    public event Action Started;
    public event Action Defeat;
    public event Action Victory;

    public void ResetSettings()
    {
        _collector.ResetSettings();
    }

    public void StartGame()
    {
        IsActive = true;
        Started?.Invoke();
    }

    public void VictoryGame()
    {
        IsActive = false;
        Victory?.Invoke();
        Debug.Log("Мини-игра пройдена.");
    }

    public void DefeatGame()
    {
        IsActive = false;
        Defeat?.Invoke();
        Debug.Log("Мини-игра проиграна.");
    }
}
