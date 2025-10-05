using UnityEngine;

public class PauseMenu : Window
{
    private void OnEnable()
    {
        _game.Leaved += OffMenu;
    }

    private void OnDisable()
    {
        _game.Leaved -= OffMenu;
    }

    private void Awake()
    {
        OffMenu();
    }
}
