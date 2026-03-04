using UnityEngine;
using YG;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderBoard;
    [SerializeField] private GameTimer _timer;

    private void OnEnable()
    {
        _timer.Stoped += SetNewScore;
    }

    private void OnDisable()
    {
        _timer.Stoped -= SetNewScore;
    }

    private void SetNewScore(int score)
    {
        YG2.SetLeaderboard(_leaderBoard.name, score);
        _leaderBoard.UpdateLB();
    }
}
