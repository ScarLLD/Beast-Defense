using Game.Scripts.Lifecycle;
using Game.Scripts.UI.Menu;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

namespace Game.Scripts.LeaderBoard
{
    public class ScoreReader : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private GameObject _recordIdentifier;
        [SerializeField] private GameTimer _timer;
        [SerializeField] private LeaderboardYG _leaderboard;
        [SerializeField] private LeaderBoardMenu _leaderboardMenu;

        private LBData _lbData;
        private float _pendingScore;

        private void Awake()
        {
            _pendingScore = 0;
        }

        private void OnEnable()
        {
            YG2.onGetLeaderboard += OnLeaderboardDataReceived;
            _timer.Stopped += OnTimerStopped;

            YG2.GetLeaderboard(_leaderboard.nameLB);
        }

        private void OnDisable()
        {
            YG2.onGetLeaderboard -= OnLeaderboardDataReceived;
            _timer.Stopped -= OnTimerStopped;
            _pendingScore = 0;
        }

        private void OnTimerStopped(float time)
        {
            var totalSeconds = (int)time;
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;

            var fractionalPart = time - totalSeconds;
            var hundredthsOfSecond = (int)(fractionalPart * 100);

            var formattedTime = $"{minutes}:{seconds}.{hundredthsOfSecond}";

            _scoreText.text = formattedTime;

            if (TryGetScore(out var loadedTime, out var isEmptyScore))
                _recordIdentifier.SetActive(loadedTime > time || isEmptyScore);
            else
                _recordIdentifier.SetActive(false);

            SetNewScore(time);
        }

        private void SetNewScore(float newScore)
        {
            if (newScore <= 0) return;

            if (_lbData == null)
            {
                _pendingScore = newScore;
                YG2.GetLeaderboard(_leaderboard.nameLB);
                return;
            }

            SubmitScoreInternal(newScore);
        }

        private void SubmitScoreInternal(float newScore)
        {
            var scoreRetrieved = TryGetScore(out var loadedScore, out _);

            if (scoreRetrieved)
            {
                if (newScore < loadedScore)
                    YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }
            else
            {
                YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }

            _leaderboard.UpdateLB();
        }

        private void OnLeaderboardDataReceived(LBData lbData)
        {
            if (lbData.technoName != _leaderboard.nameLB) return;

            _lbData = lbData;

            if (!(_pendingScore > 0)) return;

            SubmitScoreInternal(_pendingScore);
            _pendingScore = 0;
        }

        private bool TryGetScore(out float score, out bool isEmptyScore)
        {
            isEmptyScore = false;
            score = 0;

            if (_lbData == null) return false;

            if (_lbData.entries == InfoYG.NO_DATA)
            {
                isEmptyScore = true;
                return false;
            }

            foreach (var player in _lbData.players)
            {
                if (player.uniqueID != YG2.player.id) continue;

                score = player.score / 1000f;
                return true;
            }

            return false;
        }
    }
}