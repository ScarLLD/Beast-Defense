public class PlayMenu : Window
{
    private void OnEnable()
    {
        _game.Started += OnMenu;
        _game.Leaved += OffMenu;
    }

    private void OnDisable()
    {
        _game.Started -= OnMenu;
        _game.Leaved += OffMenu;
    }

    private void Awake()
    {
        OffMenu();
    }
}
