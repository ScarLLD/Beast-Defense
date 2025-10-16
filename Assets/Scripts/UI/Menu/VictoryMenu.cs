public class VictoryMenu : Window
{
    private void OnEnable()
    {
        _game.Completed += EnableMenu;
        _game.Continued += DisableMenu;
        _game.Leaved += DisableMenu;
    }

    private void OnDisable()
    {
        _game.Completed -= EnableMenu;
        _game.Continued -= DisableMenu;
        _game.Leaved -= DisableMenu;
    }
}
