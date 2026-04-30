using System;
using TMPro;
using UnityEngine;
using YG;
using YG.Utils.LB;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboard;
    [SerializeField] private LeaderBoardMenu _leaderboardMenu;
    [SerializeField] private GameTimer _timer;

    private LBData _lbData;

    private void Awake()
    {
        _leaderboard.UpdateLB();
    }

    private void OnEnable()
    {
        YG2.onGetLeaderboard += OnLeaderboardDataReceived;
        _leaderboardMenu.Opened += _leaderboard.UpdateLB;
        _timer.Stopped += SetNewScore;
    }

    private void OnDisable()
    {
        YG2.onGetLeaderboard -= OnLeaderboardDataReceived;
        _leaderboardMenu.Opened -= _leaderboard.UpdateLB;
        _timer.Stopped -= SetNewScore;
    }

    private void SetNewScore(float newScore)
    {
        if (newScore <= 0)
            return;

        if (TryGetScore(out float loadedScore) && loadedScore > 0 && loadedScore > newScore)
        {
            YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            _leaderboard.UpdateLB();
        }
    }

    private void OnLeaderboardDataReceived(LBData lbData)
    {
        if (lbData.technoName == _leaderboard.nameLB)
        {
            _lbData = lbData;
        }
    }

    public bool TryGetScore(out float score)
    {
        score = 0;

        if (_lbData == null)
            return false;

        if (_lbData.entries == InfoYG.NO_DATA)
        {
            return false;
        }

        foreach (var player in _lbData.players)
        {
            if (player.uniqueID == YG2.player.id)
            {
                score = player.score / 1000;
                return true;
            }
        }

        return false;
    }
}
