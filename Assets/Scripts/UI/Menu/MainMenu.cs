using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Window
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _leaderboardButton;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _playButton.onClick.AddListener(OnPlayButtonClick);
        _playButton.onClick.AddListener(OnPlayButtonClick);

        _game.Started += DisableMenu;
        _game.Leaved += EnableMenu;
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClick);

        _game.Started -= DisableMenu;
        _game.Leaved -= EnableMenu;
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
        //
    }
}
