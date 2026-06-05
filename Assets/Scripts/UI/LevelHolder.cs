using UnityEngine;
using YG;

public class LevelHolder : MonoBehaviour
{
    [SerializeField] private Game _game;

    private void OnEnable()
    {
        _game.Completed += IncreaseLevel;
    }

    private void OnDisable()
    {
        _game.Completed -= IncreaseLevel;
    }

    public void IncreaseLevel()
    {
        YG2.saves.LevelNumber++;
        YG2.SaveProgress();
    }
}
