using UnityEngine;
using YG;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderBoard;
    [SerializeField] private GameTimer _timer;

    private float _lastTimeScore = 0;
    private float _currentTimeScore = 0;

    private const string SCORE_KEY = "Score";

    public float GetScore => _lastTimeScore;

    private void Awake()
    {
        LoadHighScore();
        _lastTimeScore = _currentTimeScore;
    }

    private void OnEnable()
    {
        _timer.Stopped += SetNewScore;
    }

    private void OnDisable()
    {
        _timer.Stopped -= SetNewScore;
    }

    private void SetNewScore(float timeScore)
    {
        _lastTimeScore = timeScore;

        if (_currentTimeScore == 0 || timeScore < _currentTimeScore)
        {
            _currentTimeScore = timeScore;
            SaveHighScore();
            YG2.SetLBTimeConvert(_leaderBoard.name, _currentTimeScore);
            _leaderBoard.UpdateLB();
        }
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetFloat(SCORE_KEY, _currentTimeScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        if (PlayerPrefs.HasKey(SCORE_KEY))
        {
            _currentTimeScore = PlayerPrefs.GetFloat(SCORE_KEY);
        }
        else
        {
            _currentTimeScore = 0;
        }
    }
}
