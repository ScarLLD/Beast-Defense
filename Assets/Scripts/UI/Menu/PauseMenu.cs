public class PauseMenu : Window
{
    bool _isActive = false;

    public void SwitchVisible()
    {
        _isActive = !_isActive;

        SwitchVisible(_isActive);
    }
}
