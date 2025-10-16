public class MainMenu : Window
{
    private void OnEnable()
    {
        _game.Started += DisableMenu;
        _game.Leaved += EnableMenu;
    }

    private void OnDisable()
    {
        _game.Started -= DisableMenu;
        _game.Leaved -= EnableMenu;
    }

    private void Awake()
    {
        EnableMenu();
    }
}
