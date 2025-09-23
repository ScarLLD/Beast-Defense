public class GameOverMenu : Window
{
    private void OnEnable()
    {
        _game.Over += OnGameOver;
    }

    private void OnDisable()
    {
        _game.Over -= OnGameOver;
    }

    private void Awake()
    {
        SwitchVisible(false);
    }

    private void OnGameOver()
    {
        SwitchVisible(true);
    }
}
