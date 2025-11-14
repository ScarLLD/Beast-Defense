using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPlaceMenu : Window
{
    [SerializeField] private Game _game;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private DeathModule _deathModule;
    [SerializeField] private PlaceSpawner _placeSpawner;
    [SerializeField] private int _placePrice = 2;
    [SerializeField] private TMP_Text _placePriceText;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Image _buyButtonImage;
    [SerializeField] private Image _buyButtonIconImane;

    private Color _notEnoughMoneyColor = Color.red;
    private Color _enoughMoneyColor = Color.green;
    private readonly float _notEnoughMoneyAlpha = 0.4f;
    private readonly float _enoughMoneyAlpha = 1f;

    private void Awake()
    {
        DisableMenu();
    }

    private void OnEnable()
    {
        _deathModule.BeastDie += OnBeastDieClick;
        _wallet.CountChanged += SetCurrentPriceView;

        _buyButton.onClick.AddListener(OnBuyButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDisable()
    {
        _deathModule.BeastDie -= OnBeastDieClick;
        _wallet.CountChanged -= SetCurrentPriceView;

        _buyButton.onClick.RemoveListener(OnBuyButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnBeastDieClick()
    {
        if (_placeSpawner.PlacesIncreased)
        {
            _game.GameOver();
        }
        else
        {
            EnableMenu();
            _game.StopGameTime();
        }

    }

    private void OnBuyButtonClick()
    {
        _game.ContinueGameTime();
        _game.RestartGame();
        _wallet.DecreaseMoney(_placePrice);
        _placeSpawner.IncreasePlace();
        DisableMenu();
    }

    private void OnExitButtonClick()
    {
        _game.ContinueGameTime();
        _game.GameOver();
        DisableMenu();
    }

    private void SetCurrentPriceView()
    {
        _placePriceText.text = _placePrice.ToString();

        if (_wallet.CanAfford(_placePrice))
        {
            _placePriceText.color = _enoughMoneyColor;
            _buyButton.interactable = true;

            Color buttonColor = _buyButtonImage.color;
            buttonColor.a = _enoughMoneyAlpha;
            _buyButtonImage.color = buttonColor;

            Color iconColor = _buyButtonIconImane.color;
            iconColor.a = _enoughMoneyAlpha;
            _buyButtonIconImane.color = iconColor;

        }
        else
        {
            _placePriceText.color = _notEnoughMoneyColor;
            _buyButton.interactable = false;

            Color buttonColor = _buyButtonImage.color;
            buttonColor.a = _notEnoughMoneyAlpha;
            _buyButtonImage.color = buttonColor;

            Color iconColor = _buyButtonIconImane.color;
            iconColor.a = _notEnoughMoneyAlpha;
            _buyButtonIconImane.color = iconColor;

        }
    }
}
