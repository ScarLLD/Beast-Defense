using Game.Scripts.MiniGameCore;
using Game.Scripts.Shop;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace Game.Scripts.UI.Menu
{
    public class MainMenu : Window
    {
        [SerializeField] private ShopMenu _shop;
        [SerializeField] private MapGenerator.Game _game;
        [SerializeField] private MiniGame _miniGame;
        [SerializeField] private MiniGameSequenceAnimator _miniGameSequenceAnimator;
        [SerializeField] private LeaderBoardMenu _leaderBoardMenu;
        [SerializeField] private IncreaseHeartMenu _increaseHeartMenu;
        [SerializeField] private ShopMenu _shopMenu;
        [SerializeField] private GameHeart _gameHeart;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _leaderboardButton;
        [SerializeField] private Button _miniGameStartButton;

        private bool _isGameReadySent;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(OnPlayButtonClick);
            _shopButton.onClick.AddListener(OnShopButtonClick);
            _leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClick);

            _game.Started += OnGameStarted;
            _game.Leaved += OnGameLeaved;

            _miniGameStartButton.onClick.AddListener(OnMiniGameStarted);
            _miniGameSequenceAnimator.Closed += OnMiniGameLeaved;

            _shop.Opened += OnShopOpened;
            _shop.Closed += OnShopClosed;

            _leaderBoardMenu.Opened += OnLeaderBoardOpened;
            _leaderBoardMenu.Closed += OnLeaderBoardClosed;
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClick);
            _shopButton.onClick.RemoveListener(OnShopButtonClick);
            _leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClick);

            _game.Started -= OnGameStarted;
            _game.Leaved -= OnGameLeaved;

            _miniGameStartButton.onClick.RemoveListener(OnMiniGameStarted);
            _miniGameSequenceAnimator.Closed -= OnMiniGameLeaved;

            _shop.Opened -= OnShopOpened;
            _shop.Closed -= OnShopClosed;

            _leaderBoardMenu.Opened -= OnLeaderBoardOpened;
            _leaderBoardMenu.Closed -= OnLeaderBoardClosed;
        }

        private async void Awake()
        {
            EnableMenu();

            await Task.Yield();

            SendGameReady();
        }

        private void SendGameReady()
        {
            if (_isGameReadySent)
                return;

            _isGameReadySent = true;

            YG2.GameReadyAPI();
            YG2.GameplayStart();
        }

        private void OnPlayButtonClick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            if (_gameHeart.IsPossibleDecrease)
                _game.Begin();
            else
                _gameHeart.PlayShakeAnimation();
        }

        private void OnShopButtonClick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            _shopMenu.Open();
        }

        private void OnLeaderBoardButtonClick()
        {
            CallClickEvent();

            if (_increaseHeartMenu.IsActive)
                return;

            _leaderBoardMenu.Open();
        }

        private void OnGameStarted() => DisableMenu();

        private void OnGameLeaved() => EnableMenu();

        private void OnMiniGameStarted() => DisableMenu();

        private void OnMiniGameLeaved() => EnableMenu();

        private void OnLeaderBoardOpened() => DisableMenu();

        private void OnLeaderBoardClosed() => EnableMenu();

        private void OnShopOpened() => DisableMenu();

        private void OnShopClosed() => EnableMenu();
    }
}