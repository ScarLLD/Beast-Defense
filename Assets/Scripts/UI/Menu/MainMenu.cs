public class MainMenu : Window
{   
    private void OnEnable()
    {
        _game.Started += OffMenu;
    }

    private void OnDisable()
    {
        _game.Started -= OffMenu;
    }

    private void Awake()
    {
        SwitchVisible(true);
    }

    private void OffMenu()
    {
        SwitchVisible(false);
    }
}
