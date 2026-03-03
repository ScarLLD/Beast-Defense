using UnityEngine;
using UnityEngine.UI;

public class IncreaseHeartMenu : Window
{
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
        _AdvButton.onClick.AddListener(OnAdvButtonClick);
        _closeMenuButton.onClick.AddListener(OnCloseMenuButtonClick);
    }

    private void OnDisable()
    {
        _gameHeart.Devastated -= OnGameHeartDevastated;
        _AdvButton.onClick.RemoveListener(OnAdvButtonClick);
        _closeMenuButton.onClick.RemoveListener(OnCloseMenuButtonClick);
    }

    private void OnGameHeartDevastated()
    {
        EnableMenu();
    }

    private void OnAdvButtonClick()
    {

        DisableMenu();
    }

    private void OnCloseMenuButtonClick()
    {

        DisableMenu();
    }

}