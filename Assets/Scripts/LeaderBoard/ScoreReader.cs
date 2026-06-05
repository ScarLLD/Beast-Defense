using UnityEngine;
using YG;
using YG.Utils.LB;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboard;
    [SerializeField] private LeaderBoardMenu _leaderboardMenu;
    [SerializeField] private GameTimer _timer;

    private LBData _lbData;
    private float _pendingScore = 0;

    private void Awake()
    {
        _leaderboard.UpdateLB();
    }

    private void OnEnable()
    {
        YG2.onGetLeaderboard += OnLeaderboardDataReceived;
        _timer.Stopped += SetNewScore;
    }

    private void OnDisable()
    {
        YG2.onGetLeaderboard -= OnLeaderboardDataReceived;
        _timer.Stopped -= SetNewScore;
        _pendingScore = 0;
    }

    private void SetNewScore(float newScore)
    {
        if (newScore <= 0)
            return;

        if (_lbData == null)
            return;

        bool scoreRetrieved = TryGetScore(out float loadedScore, out bool isEmptyScore);

        if (scoreRetrieved)
        {
            if (isEmptyScore)
            {
                YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }
            else if (newScore < loadedScore)
            {
                YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }
        }
        else
        {
            YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
        }

        _leaderboard.UpdateLB();
    }

    private void OnLeaderboardDataReceived(LBData lbData)
    {
        if (lbData.technoName == _leaderboard.nameLB)
        {
            _lbData = lbData;

            if (_pendingScore > 0)
            {
                SetNewScore(_pendingScore);
                _pendingScore = 0;
            }
        }
    }

    public bool TryGetScore(out float score, out bool isEmptyScore)
    {
        isEmptyScore = false;
        score = 0;

        if (_lbData == null)
        {
            return false;
        }

        if (_lbData.entries == InfoYG.NO_DATA)
        {
            isEmptyScore = true;
            return false;
        }

        foreach (var player in _lbData.players)
        {
            if (player.uniqueID == YG2.player.id)
            {
                score = player.score / 1000f;
                return true;
            }
        }

        return false;
    }
}