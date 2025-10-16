public class PlayMenu : Window
{
    private void OnEnable()
    {
        _game.Started += EnableMenu;
        _game.Leaved += DisableMenu;
    }

    private void OnDisable()
    {
        _game.Started -= EnableMenu;
        _game.Leaved += DisableMenu;
    }

    private void Awake()
    {
        DisableMenu();
    }
}
