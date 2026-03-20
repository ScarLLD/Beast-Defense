using UnityEngine;
using UnityEngine.UI;

public class BeastCollector : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private MGBeastSpawner _beastSpawner;

    private int _beastCollectedCount = 0;
    private int _maxBeastCollectedCount = 10;

    private void Awake()
    {
        _maxBeastCollectedCount = _beastSpawner.MaxBeastCount;
    }

    public void IncreaseBeastCount()
    {
        _beastCollectedCount += 1;
        DisplayCount();

        if (_beastCollectedCount == _maxBeastCollectedCount)
            _miniGame.VictoryGame();
    }

    public void ResetSettings()
    {
        _beastCollectedCount = 0;
        DisplayCount();
    }

    private void DisplayCount()
    {
        _text.text = $"{_beastCollectedCount}/{_maxBeastCollectedCount}";
    }
}
