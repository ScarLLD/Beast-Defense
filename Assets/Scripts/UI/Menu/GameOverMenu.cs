using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Window
{
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _game.Over += EnableMenu;
        _game.Transited += DisableMenu;

        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _game.Over -= EnableMenu;
        _game.Transited -= DisableMenu;

        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void Awake()
    {
        DisableMenu();
    }

    private void OnRestartButtonClick()
    {
        _game.RestartGame();
    }

    private void OnExitButtonClick()
    {
        _game.LeaveGame();
    }
}
