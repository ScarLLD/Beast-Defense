using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SliderLevelViewer : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private TMP_Text _sliderLevelViewer;

    private int _currentLevelNumber;

    private void OnEnable()
    {
        _game.Completed += IncreaseLevelNumber;
    }

    private void OnDisable()
    {
        _game.Completed -= IncreaseLevelNumber;
    }

    private void Start()
    {
        _currentLevelNumber = 1;
        DisplayText();
    }

    private void DisplayText()
    {
        _sliderLevelViewer.text = $"Уровень {_currentLevelNumber}";
    }

    public void IncreaseLevelNumber()
    {
        _currentLevelNumber++;
        DisplayText();
    }
}
