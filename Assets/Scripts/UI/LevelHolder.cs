using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    [SerializeField] private Game _game;

    private int _levelNumber = 1;

    public int GetLevelNumber => _levelNumber;

    public event Action LevelChanged;

    private void OnEnable()
    {
        _game.Completed += IncreaseLevel;
    }

    private void OnDisable()
    {
        _game.Completed -= IncreaseLevel;
    }

    public void IncreaseLevel()
    {
        _levelNumber++;
        LevelChanged?.Invoke();
    }
}
