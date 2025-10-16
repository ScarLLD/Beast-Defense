using UnityEngine;

public class PauseMenu : Window
{
    private void OnEnable()
    {
        _game.Leaved += DisableMenu;
        _game.Restarted += DisableMenu;
    }

    private void OnDisable()
    {
        _game.Leaved -= DisableMenu;
        _game.Restarted -= DisableMenu;
    }

    private void Awake()
    {
        DisableMenu();
    }
}
