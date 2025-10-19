using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : Window
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _game.Completed += EnableMenu;
        _game.Transited += DisableMenu;

        _continueButton.onClick.AddListener(OnContinuedButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _game.Completed -= EnableMenu;
        _game.Transited -= DisableMenu;

        _continueButton.onClick.RemoveListener(OnContinuedButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnContinuedButtonClick()
    {
        _game.ContinueGame();
    }

    private void OnExitButtonClick()
    {
        _game.LeaveGame();
    }
}
