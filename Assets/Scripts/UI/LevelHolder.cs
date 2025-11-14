using System;
using UnityEngine;

public class LevelHolder : MonoBehaviour
{
    [SerializeField] private Game _game;

    private const string LEVEL_KEY = "LevelNumber";

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

    private void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        _levelNumber = PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }

    private void SaveLevel()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, _levelNumber);
        PlayerPrefs.Save();
    }

    public void IncreaseLevel()
    {
        _levelNumber++;
        SaveLevel();
        LevelChanged?.Invoke();
    }
}
