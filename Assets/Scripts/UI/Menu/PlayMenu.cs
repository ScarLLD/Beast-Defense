public class PlayMenu : Window
{
    private void OnEnable()
    {
        _game.Started += OnMenu;
    }

    private void OnDisable()
    {
        _game.Started -= OnMenu;
    }

    private void Awake()
    {
        SwitchVisible(false);
    }

    private void OnMenu()
    {
        SwitchVisible(true);
    }
}
