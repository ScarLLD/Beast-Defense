using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Window
{
    [SerializeField] private Game _game;
    [SerializeField] private LeaderBoardMenu _leaderBoardMenu;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _leaderboardButton;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _leaderboardButton.onClick.AddListener(OnLeaderBoardButtonClick);

        _game.Started += DisableMenu;
        _game.Leaved += EnableMenu;

        _leaderBoardMenu.Opened += DisableMenu;
        _leaderBoardMenu.Closed += EnableMenu;
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClick);
        _leaderboardButton.onClick.RemoveListener(OnLeaderBoardButtonClick);

        _game.Started -= DisableMenu;
        _game.Leaved -= EnableMenu;

        _leaderBoardMenu.Opened -= DisableMenu;
        _leaderBoardMenu.Closed -= EnableMenu;
    }

    private void Awake()
    {
        EnableMenu();
    }

    private void OnPlayButtonClick()
    {
        _game.StartGame();
    }

    private void OnShopButtonClick()
    {
        //
    }

    private void OnLeaderBoardButtonClick()
    {
        _leaderBoardMenu.Open();
    }
}
