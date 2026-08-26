using Game.Scripts.LifeCycle;
using Game.Scripts.MapGenerator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menu
{
    public class BuyPlaceMenu : Window
    {
        private const float NOT_ENOUGH_MONEY_ALPHA = 0.4f;
        private const float ENOUGH_MONEY_ALPHA = 1f;

        private readonly Color _notEnoughMoneyColor = Color.red;
        private readonly Color _enoughMoneyColor = Color.green;
        
        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private Wallet _wallet;
        [SerializeField] private DeathModule _deathModule;
        [SerializeField] private PlaceSpawner _placeSpawner;
        [SerializeField] private int _placePrice = 2;
        [SerializeField] private TMP_Text _placePriceText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Image _buyButtonImage;
        [SerializeField] private Image _buyButtonIconImage;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _deathModule.BeastDied += OnBeastDied;
            _wallet.CountChanged += OnWalletMoneyCountChanged;

            _buyButton.onClick.AddListener(OnBuyButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            _deathModule.BeastDied -= OnBeastDied;
            _wallet.CountChanged -= OnWalletMoneyCountChanged;

            _buyButton.onClick.RemoveListener(OnBuyButtonClick);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnBeastDied()
        {
            if (_placeSpawner.PlacesIncreased)
            {
                _game.Over();
            }
            else
            {
                EnableMenu();
                _game.StopTime();
            }
        }

        private void OnBuyButtonClick()
        {
            CallClickEvent();

            _game.ContinueTime();
            _game.Restart();
            _wallet.DecreaseMoney(_placePrice);
            _placeSpawner.IncreasePlace();
            DisableMenu();
        }

        private void OnExitButtonClick()
        {
            CallClickEvent();

            _game.ContinueTime();
            _game.Over();
            DisableMenu();
        }

        private void OnWalletMoneyCountChanged()
        {
            _placePriceText.text = _placePrice.ToString();

            if (Wallet.CanAfford(_placePrice))
            {
                _placePriceText.color = _enoughMoneyColor;
                _buyButton.interactable = true;

                var buttonColor = _buyButtonImage.color;
                buttonColor.a = ENOUGH_MONEY_ALPHA;
                _buyButtonImage.color = buttonColor;

                var iconColor = _buyButtonIconImage.color;
                iconColor.a = ENOUGH_MONEY_ALPHA;
                _buyButtonIconImage.color = iconColor;

            }
            else
            {
                _placePriceText.color = _notEnoughMoneyColor;
                _buyButton.interactable = false;

                var buttonColor = _buyButtonImage.color;
                buttonColor.a = NOT_ENOUGH_MONEY_ALPHA;
                _buyButtonImage.color = buttonColor;

                var iconColor = _buyButtonIconImage.color;
                iconColor.a = NOT_ENOUGH_MONEY_ALPHA;
                _buyButtonIconImage.color = iconColor;

            }
        }
    }
}