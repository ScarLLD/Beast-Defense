using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Window
{
    [SerializeField] private Game _game;
    [SerializeField] private Adv _adv;
    [SerializeField] private GameHeart _gameHeart;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _advRegenerateLevelButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private TMP_Text _regenerateText;

    private float _advButtonAlphaPressedColor = 0.5f;

    private void OnEnable()
    {
        _game.Over += EnableMenu;
        _game.Transited += DisableMenu;

        _advRegenerateLevelButton.onClick.AddListener(OnRegenerateButtonCLick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);

        if (_gameHeart.IsPossibleDecrease)
            EnableAdvButton();
    }

    private void OnDisable()
    {
        _game.Over -= EnableMenu;
        _game.Transited -= DisableMenu;

        _advRegenerateLevelButton.onClick.RemoveListener(OnRegenerateButtonCLick);
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void Awake()
    {
        DisableMenu();
    }

    private void OnRegenerateButtonCLick()
    {
        if (_gameHeart.IsPossibleDecrease)
            _adv.RegenerateLevelAdvShow();
        else
            _gameHeart.PlayShakeAnimation();

        DisableAdvButton();
    }

    private void OnRestartButtonClick()
    {
        if (_gameHeart.IsPossibleDecrease)
            _game.Restart();
        else
            _gameHeart.PlayShakeAnimation();
    }

    private void OnExitButtonClick()
    {
        _game.Leave();
    }

    private void DisableAdvButton()
    {
        _advRegenerateLevelButton.interactable = false;
        _regenerateText.alpha = _advButtonAlphaPressedColor;
    }

    private void EnableAdvButton()
    {
        _advRegenerateLevelButton.interactable = true;
        _regenerateText.alpha = 1f;
    }
}
