using System;
using TMPro;
using Unity.VisualScripting;
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

    [SerializeField] private Button _advDoubleRewardButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _exitButton;

    private float _advButtonAlphaPressedColor = 0.5f;
    private int _rewardMultiple = 2;

    private void Awake()
    {
        DisableMenu();
    }

    private void OnEnable()
    {
        _game.Completed += OnGameCompleted;
        _game.Transited += DisableMenu;

        _advDoubleRewardButton.onClick.AddListener(OnDoubleRewardButtonClick);
        _continueButton.onClick.AddListener(OnContinuedButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _game.Completed -= OnGameCompleted;
        _game.Transited -= DisableMenu;

        _advDoubleRewardButton.onClick.RemoveListener(OnDoubleRewardButtonClick);
        _continueButton.onClick.RemoveListener(OnContinuedButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void EnableAdvButton()
    {
        _advDoubleRewardButton.interactable = true;

        Color iconColor = _iconImage.color;
        iconColor.a = 1f;
        _iconImage.color = iconColor;

        _doubleRewardText.alpha = 1f;
        _doubleRewardMultipleText.alpha = 1f;
    }

    private void DisableAdvButton()
    {
        _advDoubleRewardButton.interactable = false;

        Color iconColor = _iconImage.color;
        iconColor.a = _advButtonAlphaPressedColor;
        _iconImage.color = iconColor;

        _doubleRewardText.alpha = _advButtonAlphaPressedColor;
        _doubleRewardMultipleText.alpha = _advButtonAlphaPressedColor;
    }

    private void OnDoubleRewardButtonClick()
    {
        _adv.DoubleRewardAdvShow();
        DisableAdvButton();
        DisplayNewRaward();
    }

    private void DisplayNewRaward()
    {
        _totalRewardText.text = $"+{_wallet.GetRewardMoneyCount() * _rewardMultiple}";
        _totalRewardText.color = Color.yellow;
    }

    private void OnGameCompleted()
    {
        EnableAdvButton();
        EnableMenu();
        _totalRewardText.color = Color.green;
        _totalRewardText.text = $"+{_wallet.GetRewardMoneyCount()}";
    }

    private void OnContinuedButtonClick()
    {
        _game.Continue();
    }

    private void OnExitButtonClick()
    {
        _game.Leave();
    }
}
