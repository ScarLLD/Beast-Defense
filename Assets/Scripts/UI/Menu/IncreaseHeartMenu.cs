using UnityEngine;
using UnityEngine.UI;

public class IncreaseHeartMenu : Window
{
    [SerializeField] private Adv _adv;
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

        _miniGameButton.onClick.AddListener(OnMiniGameButtonClick);
        _AdvButton.onClick.AddListener(OnAdvButtonClick);
        _closeMenuButton.onClick.AddListener(OnCloseMenuButtonClick);
    }

    private void OnDisable()
    {
        _gameHeart.Devastated -= OnGameHeartDevastated;

        _miniGameButton.onClick.RemoveListener(OnMiniGameButtonClick);
        _AdvButton.onClick.RemoveListener(OnAdvButtonClick);
        _closeMenuButton.onClick.RemoveListener(OnCloseMenuButtonClick);
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

        DisableMenu();
    }

    private void OnCloseMenuButtonClick()
    {

        DisableMenu();
    }

}