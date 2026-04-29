using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : Window
{
    [SerializeField] private Game _game;
    [SerializeField] private Adv _adv;
    [SerializeField] private GameHeart _gameHeart;
    [SerializeField] private IncreaseHeartMenu _increaseHeartMenu;
    [SerializeField] private TMP_Text _regenerateText;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _advRegenerateLevelButton;
    [SerializeField] private Button _exitButton;

    [Header("Mini-Game")]
    [SerializeField] private Button _miniGameStartButton;

    private readonly float _advButtonAlphaPressedColor = 0.5f;

    private void OnEnable()
    {
        _game.Loss += OnGameLoss;
        _game.Transited += DisableMenu;

        _advRegenerateLevelButton.onClick.AddListener(OnRegenerateButtonCLick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
        _exitButton.onClick.AddListener(OnExitButtonClick);

        _miniGameStartButton.onClick.AddListener(OnMiniGameStartButtonClick);

        if (_gameHeart.IsPossibleDecrease)
            EnableAdvButton();
    }

    private void OnMiniGameStartButtonClick()
    {
        if (IsActive)
            _game.Leave();
    }

    private void OnDisable()
    {
        _game.Loss -= OnGameLoss;
        _game.Transited -= DisableMenu;

        _advRegenerateLevelButton.onClick.RemoveListener(OnRegenerateButtonCLick);
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
        _exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void Awake()
    {
        DisableMenu();
    }

    private void OnGameLoss()
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
