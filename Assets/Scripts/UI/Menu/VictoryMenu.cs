using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : Window
{
    [SerializeField] private Adv _adv;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private Game _game;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _totalRewardText;
    [SerializeField] private TMP_Text _doubleRewardText;
    [SerializeField] private TMP_Text _doubleRewardMultipleText;
    [SerializeField] private float _advButtonAlphaPressedColor = 0.5f;

    [SerializeField] private Button _doubleRewardButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    private void Awake()
    {
        DisableMenu();
    }

    private void OnEnable()
    {
        _game.Completed += OnGameCompleted;
        _game.Transited += DisableMenu;

        _adv.Doubled += OnRewardDoubled;

        _continueButton.onClick.AddListener(OnContinuedButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
        EnableAdvButton();

    }

    private void EnableAdvButton()
    {
        _doubleRewardButton.interactable = true;

        Color iconColor = _iconImage.color;
        iconColor.a = 1f;
        _iconImage.color = iconColor;

        _doubleRewardText.alpha = 1f;
        _doubleRewardMultipleText.alpha = 1f;
    }

    private void DisableAdvButton()
    {
        _doubleRewardButton.interactable = false;

        Color iconColor = _iconImage.color;
        iconColor.a = _advButtonAlphaPressedColor;
        _iconImage.color = iconColor;

        _doubleRewardText.alpha = _advButtonAlphaPressedColor;
        _doubleRewardMultipleText.alpha = _advButtonAlphaPressedColor;
    }

    private void OnDisable()
    {
        _game.Completed -= OnGameCompleted;
        _game.Transited -= DisableMenu;

        _adv.Doubled -= OnRewardDoubled;

        _continueButton.onClick.RemoveListener(OnContinuedButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnRewardDoubled()
    {
        DisableAdvButton();

        _totalRewardText.text = $"+{_wallet.GetRewardMoneyCount() * 2}";
        _totalRewardText.color = Color.yellow;
    }

    private void OnGameCompleted()
    {
        EnableMenu();
        _totalRewardText.color = Color.green;
        _totalRewardText.text = $"+{_wallet.GetRewardMoneyCount()}";
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
