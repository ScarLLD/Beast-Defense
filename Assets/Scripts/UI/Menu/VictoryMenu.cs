using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : Window
{
    [SerializeField] private Wallet _wallet;
    [SerializeField] private Game _game;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    private void OnEnable()
    {
        _game.Completed += OnGameCompleted;
        _game.Transited += DisableMenu;

        _continueButton.onClick.AddListener(OnContinuedButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _game.Completed -= OnGameCompleted;
        _game.Transited -= DisableMenu;

        _continueButton.onClick.RemoveListener(OnContinuedButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnGameCompleted()
    {
        EnableMenu();
        _text.color = Color.green;
        _text.text = $"{_wallet.GetMoneyCount()} + {_wallet.GetRewardMoneyCount()}";
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
