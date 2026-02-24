using UnityEngine;
using UnityEngine.UI;
using YG;

public class PauseMenu : Window
{
    [SerializeField] private Game _game;
    [SerializeField] private GameOptions _gameOptions;

    [Header("Buttons")]
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _closePauseButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _vibroButton;

    private void OnEnable()
    {
        _pauseButton.onClick.AddListener(OnPauseButtonClick);
        _closePauseButton.onClick.AddListener(OnClosePauseButtonClick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);

        _musicButton.onClick.AddListener(_gameOptions.ToggleMusic);
        _soundButton.onClick.AddListener(_gameOptions.ToggleSound);
        _vibroButton.onClick.AddListener(_gameOptions.ToggleVibration);

        YG2.onFocusWindowGame += OnFocusWindowGame;
    }

    private void OnDisable()
    {
        _pauseButton.onClick.RemoveListener(OnPauseButtonClick);
        _closePauseButton.onClick.RemoveListener(OnClosePauseButtonClick);
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);

        _musicButton.onClick.RemoveListener(_gameOptions.ToggleMusic);
        _soundButton.onClick.RemoveListener(_gameOptions.ToggleSound);
        _vibroButton.onClick.RemoveListener(_gameOptions.ToggleVibration);

        YG2.onFocusWindowGame -= OnFocusWindowGame;
    }

    private void Awake()
    {
        DisableMenu();
    }

    private void OnApplicationFocus(bool focus)
    {
        OnPauseButtonClick();
    }

    private void OnFocusWindowGame(bool focus)
    {


        if (focus)
            OnClosePauseButtonClick();
        else
            OnPauseButtonClick();
    }

    private void OnPauseButtonClick()
    {
        if (_game.IsPlaying)
        {
            EnableMenu();
            YG2.PauseGame(true, true, true, false, false);
        }
    }

    private void OnRestartButtonClick()
    {
        DisableMenu();
        YG2.PauseGame(false);
        _game.Restart();
    }

    private void OnExitButtonClick()
    {
        DisableMenu();
        YG2.PauseGame(false);
        _game.FastLeave();
    }

    private void OnClosePauseButtonClick()
    {
        DisableMenu();
        YG2.PauseGame(false, false, false, false, false);
    }
}
