using UnityEngine;

public class MiniGameSequencer : MonoBehaviour
{
    [SerializeField] private SnakeSpawner _snakeSpawner;
    [SerializeField] private BeastSpawner _beastSpawner;

    [Header("MiniGameSettings")]
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private GameObject _gameObjectsParent;
    [SerializeField] private MiniGameOpenAnimator _animator;
    [SerializeField] private MGBeastSpawner _mgBeastSpawner;
    [SerializeField] private MGSnakeSpawner _mgSnakeSpawner;

    public void Launch()
    {
        InitializeSkins();
        _gameObjectsParent.SetActive(true);

        _animator.StartAnimation();
        _miniGame.ResetSettings();
        _miniGame.StartGame();
    }

    private void InitializeSkins()
    {
        var _snakeSkin = _snakeSpawner.GetCurrentSkin;
        var _beastSkin = _beastSpawner.GetCurrentSkin;

        if (_beastSkin != null)
            _mgBeastSpawner.InitializeSkin(_beastSkin.Model);

        if (_snakeSkin != null)
            _mgSnakeSpawner.InitializeSkin(_snakeSkin.Model, _snakeSkin.Color);
    }
}
