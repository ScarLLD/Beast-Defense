public class VictoryMenu : Window
{
    private void OnEnable()
    {
        _game.Ended += OnGameEnded;
    }

    private void OnDisable()
    {
        _game.Ended -= OnGameEnded;
    }

    private void Awake()
    {
        SwitchVisible(false);
    }

    private void OnGameEnded()
    {
        SwitchVisible(true);
    }
}
