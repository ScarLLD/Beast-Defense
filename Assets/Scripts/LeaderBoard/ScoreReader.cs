using UnityEngine;
using YG;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderBoard;
    [SerializeField] private GameTimer _timer;

    private float _currentTimeScore = 0;

    private const string SCORE_KEY = "Score";

    private void Awake()
    {
        LoadHighScore();
        SetNewScore(_currentTimeScore);
    }

    private void OnEnable()
    {
        _timer.Stoped += SetNewScore;
    }

    private void OnDisable()
    {
        _timer.Stoped -= SetNewScore;
    }

    private void SetNewScore(float timeScore)
    {
        _currentTimeScore = timeScore;

        if (timeScore > _currentTimeScore)
        {
            _currentTimeScore = timeScore;
            SaveHighScore();
        }

        YG2.SetLBTimeConvert(_leaderBoard.name, timeScore);
        _leaderBoard.UpdateLB();
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
