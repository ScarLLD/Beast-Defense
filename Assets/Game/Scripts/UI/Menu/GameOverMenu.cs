using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menu
{
    public class GameOverMenu : Window
    {
        private readonly float _advButtonAlphaPressedColor = 0.5f;

        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private Adv _adv;
        [SerializeField] private GameHeart _gameHeart;
        [SerializeField] private IncreaseHeartMenu _increaseHeartMenu;
        [SerializeField] private TMP_Text _regenerateText;

        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _advRegenerateLevelButton;
        [SerializeField] private Button _exitButton;

        [Header("Mini-Game")]
        [SerializeField] private Button _miniGameStartButton;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _game.Lost += OnGameLost;
            _game.Transited += OnGameTransited;

            _advRegenerateLevelButton.onClick.AddListener(OnRegenerateButtonCLick);
            _restartButton.onClick.AddListener(OnRestartButtonClick);
            _exitButton.onClick.AddListener(OnExitButtonClick);

            _miniGameStartButton.onClick.AddListener(OnMiniGameStartButtonClick);

            if (_gameHeart.IsPossibleDecrease)
                EnableAdvButton();
        }

        private void OnDisable()
        {
            _game.Lost -= OnGameLost;
            _game.Transited -= OnGameTransited;

            _advRegenerateLevelButton.onClick.RemoveListener(OnRegenerateButtonCLick);
            _restartButton.onClick.RemoveListener(OnRestartButtonClick);
            _exitButton.onClick.RemoveListener(OnExitButtonClick);

            _miniGameStartButton.onClick.RemoveListener(OnMiniGameStartButtonClick);
        }

        private void OnMiniGameStartButtonClick()
        {
            if (IsActive)
                _game.Leave();
        }

        private void OnGameLost()
        {
            EnableMenu();
            EnableAdvButton();
        }

        private void OnRegenerateButtonCLick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            if (_gameHeart.IsPossibleDecrease)
            {
                _adv.RegenerateLevelAdvShow();
                DisableAdvButton();
            }
            else
            {
                _gameHeart.PlayShakeAnimation();
            }
        }

        private void OnRestartButtonClick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            if (_gameHeart.IsPossibleDecrease)
                _game.Restart();
            else
                _gameHeart.PlayShakeAnimation();
        }

        private void OnExitButtonClick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            _game.Leave();
        }

        private void DisableAdvButton()
        {
            _advRegenerateLevelButton.interactable = false;
            _regenerateText.alpha = _advButtonAlphaPressedColor;
        }

        private void EnableAdvButton()
        {
            _advRegenerateLevelButton.interactable = true;
            _regenerateText.alpha = 1f;
        }
    }
}