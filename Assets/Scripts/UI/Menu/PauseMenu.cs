using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Window
{
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
    }

    private void Awake()
    {
        DisableMenu();
    }

    private void OnPauseButtonClick()
    {
        EnableMenu();
        _game.StopGameTime();
    }

    private void OnRestartButtonClick()
    {
        DisableMenu();
        _game.ContinueGameTime();
        _game.RestartGame();
    }

    private void OnExitButtonClick()
    {
        DisableMenu();
        _game.ContinueGameTime();
        _game.FastLeaveGame();
    }

    private void OnClosePauseButtonClick()
    {
        DisableMenu();
        _game.ContinueGameTime();
    }
}
