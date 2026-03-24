using UnityEngine;
using UnityEngine.UI;

public class IncreaseHeartMenu : Window
{
    [SerializeField] private Adv _adv;
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private MiniGameSequencer _sequencer;
    [SerializeField] private GameHeart _gameHeart;
    [SerializeField] private Button _AdvButton;
    [SerializeField] private Button _miniGameButton;
    [SerializeField] private Button _closeMenuButton;

    private void Awake()
    {
        DisableMenu();
    }

    private void OnEnable()
    {
        _gameHeart.Devastated += OnGameHeartDevastated;
        _miniGame.Started += OnMiniGameStarted;

        _miniGameButton.onClick.AddListener(OnMiniGameButtonClick);
        _AdvButton.onClick.AddListener(OnAdvButtonClick);
        _closeMenuButton.onClick.AddListener(OnCloseMenuButtonClick);
    }

    private void OnDisable()
    {
        _gameHeart.Devastated -= OnGameHeartDevastated;
        _miniGame.Started -= OnMiniGameStarted;

        _miniGameButton.onClick.RemoveListener(OnMiniGameButtonClick);
        _AdvButton.onClick.RemoveListener(OnAdvButtonClick);
        _closeMenuButton.onClick.RemoveListener(OnCloseMenuButtonClick);
    }

    private void OnMiniGameStarted()
    {
        DisableMenu();
    }

    private void OnGameHeartDevastated()
    {
        EnableMenu();
    }

    private void OnAdvButtonClick()
    {
        _adv.IncreaseGameHeartAdvShow();
        DisableMenu();
    }

    private void OnMiniGameButtonClick()
    {
        _sequencer.Launch();
        DisableMenu();
    }

    private void OnCloseMenuButtonClick()
    {

        DisableMenu();
    }

}