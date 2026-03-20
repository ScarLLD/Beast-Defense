using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Window
{
    [SerializeField] private ShopMenu _shop;
    [SerializeField] private Game _game;
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

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _shopButton.onClick.AddListener(OnShopButtonClick);
        _leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClick);

        _game.Started += DisableMenu;
        _game.Leaved += OnGameLeaved;

        _miniGameStartButton.onClick.AddListener(DisableMenu);
        _miniGameSequenceAnimator.Closed += EnableMenu;

        _shop.Opened += DisableMenu;
        _shop.Closed += EnableMenu;

        _leaderBoardMenu.Opened += DisableMenu;
        _leaderBoardMenu.Closed += EnableMenu;
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClick);
        _shopButton.onClick.RemoveListener(OnShopButtonClick);
        _leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClick);

        _game.Started -= DisableMenu;
        _game.Leaved -= OnGameLeaved;

        _miniGameStartButton.onClick.RemoveListener(DisableMenu);
        _miniGameSequenceAnimator.Closed -= EnableMenu;

        _shop.Opened -= DisableMenu;
        _shop.Closed -= EnableMenu;

        _leaderBoardMenu.Opened -= DisableMenu;
        _leaderBoardMenu.Closed -= EnableMenu;
    }

    private void Awake()
    {
        EnableMenu();
    }

    private void OnPlayButtonClick()
    {
        if (_increaseHeartMenu.IsActive)
            return;

        if (_gameHeart.IsPossibleDecrease)
            _game.Begin();
        else
            _gameHeart.PlayShakeAnimation();
    }

    private void OnShopButtonClick()
    {
        if (_increaseHeartMenu.IsActive)
            return;

        _shopMenu.Open();
    }

    private void OnLeaderBoardButtonClick()
    {
        if (_increaseHeartMenu.IsActive)
            return;

        _leaderBoardMenu.Open();
    }

    private void OnGameLeaved()
    {
        if (_miniGame.IsActive == false)
        {
            EnableMenu();
        }
    }
}
