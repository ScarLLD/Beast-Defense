public class GameOverMenu : Window
{
    private void OnEnable()
    {
        _game.Over += EnableMenu;
        _game.Restarted += DisableMenu;
        _game.Leaved += DisableMenu;
    }

    private void OnDisable()
    {
        _game.Over -= EnableMenu;
        _game.Restarted -= DisableMenu;
        _game.Leaved -= DisableMenu;
    }
}
