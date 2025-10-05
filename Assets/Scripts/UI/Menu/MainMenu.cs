public class MainMenu : Window
{
    private void OnEnable()
    {
        _game.Started += OffMenu;
        _game.Leaved += OnMenu;
    }

    private void OnDisable()
    {
        _game.Started -= OffMenu;
        _game.Leaved -= OnMenu;
    }

    private void Awake()
    {
        OnMenu();
    }
}
