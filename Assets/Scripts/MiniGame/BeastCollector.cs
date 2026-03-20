using UnityEngine;
using UnityEngine.UI;

public class BeastCollector : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private MGBeastSpawner _beastSpawner;
    [SerializeField] private MGSnake _snake;

    private int _beastCollectedCount = 0;
    private int _maxBeastCollectedCount = 10;

    public bool IsBeastsFull => _beastCollectedCount == _maxBeastCollectedCount;

    public void IncreaseBeastCount()
    {
        _beastCollectedCount += 1;
        DisplayCount();

        if (_beastCollectedCount == _maxBeastCollectedCount)
        {
            _snake.Die();
            _miniGame.VictoryGame();
        }
    }

    public void ResetSettings()
    {
        _beastCollectedCount = 0;
        DisplayCount();
    }

    public void SetNewMaxBeastCount(int count)
    {
        _maxBeastCollectedCount = count;
    }

    private void DisplayCount()
    {
        _text.text = $"{_beastCollectedCount}/{_maxBeastCollectedCount}";
    }
}
