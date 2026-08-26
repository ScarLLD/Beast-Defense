using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menu
{
    public class VictoryMenu : Window
    {
        private const float ADV_BUTTON_ALPHA_PRESSED_COLOR = 0.5f;
        private const int REWARD_MULTIPLE = 2;
        
        [SerializeField] private Adv _adv;
        [SerializeField] private Wallet _wallet;
        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _totalRewardText;
        [SerializeField] private TMP_Text _doubleRewardText;
        [SerializeField] private TMP_Text _doubleRewardMultipleText;

        [SerializeField] private Button _advDoubleRewardButton;
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _game.Completed += OnGameCompleted;
            _game.Transited += OnGameTransited;

            _adv.WinRewardDoubled += OnWinRewardDoubled;

            _advDoubleRewardButton.onClick.AddListener(OnDoubleRewardButtonClick);
            _continueButton.onClick.AddListener(OnContinuedButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            _game.Completed -= OnGameCompleted;
            _game.Transited -= OnGameTransited;

            _adv.WinRewardDoubled -= OnWinRewardDoubled;

            _advDoubleRewardButton.onClick.RemoveListener(OnDoubleRewardButtonClick);
            _continueButton.onClick.RemoveListener(OnContinuedButtonClick);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void EnableAdvButton()
        {
            _advDoubleRewardButton.interactable = true;

            var iconColor = _iconImage.color;
            iconColor.a = 1f;
            _iconImage.color = iconColor;

            _doubleRewardText.alpha = 1f;
            _doubleRewardMultipleText.alpha = 1f;
        }

        private void DisableAdvButton()
        {
            _advDoubleRewardButton.interactable = false;

            var iconColor = _iconImage.color;
            iconColor.a = ADV_BUTTON_ALPHA_PRESSED_COLOR;
            _iconImage.color = iconColor;

            _doubleRewardText.alpha = ADV_BUTTON_ALPHA_PRESSED_COLOR;
            _doubleRewardMultipleText.alpha = ADV_BUTTON_ALPHA_PRESSED_COLOR;
        }

        private void OnDoubleRewardButtonClick()
        {
            CallClickEvent();

            _adv.DoubleRewardAdvShow();
            DisableAdvButton();
        }

        private void OnWinRewardDoubled()
        {
            var doubledMoneyCount = _wallet.RewardMoneyCount * REWARD_MULTIPLE;

            _totalRewardText.text = $"+{doubledMoneyCount}";
            _totalRewardText.color = Color.yellow;
        }

        private void OnGameCompleted()
        {
            EnableAdvButton();
            EnableMenu();
            _totalRewardText.color = Color.green;
            _totalRewardText.text = $"+{_wallet.RewardMoneyCount}";
        }

        private void OnContinuedButtonClick()
        {
            CallClickEvent();

            _game.Continue();
        }

        private void OnExitButtonClick()
        {
            CallClickEvent();

            _game.Leave();
        }
    }
}