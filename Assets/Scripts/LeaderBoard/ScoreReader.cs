using System.Linq;
using TMPro;
using UnityEngine;
using YG;
using YG.Utils.LB;

public class ScoreReader : MonoBehaviour
{
    [SerializeField] private LeaderboardYG _leaderboard;
    [SerializeField] private LeaderBoardMenu _leaderboardMenu;
    [SerializeField] private GameTimer _timer;

    [SerializeField] private TMP_Text _text;

    private LBData _lbData;
    private float _pendingScore = 0; // Буфер для отложенной отправки

    // Вспомогательная функция для добавления логов в _text
    private void AddLog(string message)
    {
        if (_text != null)
        {
            _text.text += $"{message}\n";
            _text.rectTransform.anchoredPosition = new Vector2(0, 0);
        }
    }

    private void Awake()
    {
        _leaderboard.UpdateLB();
        AddLog("[Awake] Leaderboard initialized");
    }

    private void OnEnable()
    {
        YG2.onGetLeaderboard += OnLeaderboardDataReceived;
        _timer.Stopped += SetNewScore;
        AddLog("[OnEnable] Subscribed to leaderboard and timer events");
    }

    private void OnDisable()
    {
        YG2.onGetLeaderboard -= OnLeaderboardDataReceived;
        _timer.Stopped -= SetNewScore;
        _pendingScore = 0; // Очищаем буфер при отключении
        AddLog("[OnDisable] Unsubscribed from events");
    }

    private void SetNewScore(float newScore)
    {
        AddLog($"[SetNewScore] Called with newScore: {newScore} (lower is better)");

        if (newScore <= 0)
        {
            AddLog("[SetNewScore] Score is <= 0, skipping update");
            return;
        }

        if (_lbData == null)
        {
            // Данные ещё не загружены с сервера — запоминаем результат
            _pendingScore = newScore;
            AddLog("[SetNewScore] Leaderboard data not ready. Saving score for later update.");
            return;
        }

        AddLog("[SetNewScore] Attempting to get current score...");

        bool scoreRetrieved = TryGetScore(out float loadedScore, out bool isEmptyScore);
        AddLog($"[SetNewScore] TryGetScore result: {scoreRetrieved}, loadedScore: {loadedScore}, isEmptyScore: {isEmptyScore}");

        if (scoreRetrieved)
        {
            if (isEmptyScore)
            {
                AddLog("[SetNewScore] Leaderboard is empty for this player, setting initial score");
                YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }
            else if (newScore < loadedScore) // Новый результат ЛУЧШЕ (меньше), чем текущий рекорд
            {
                AddLog($"[SetNewScore] New score {newScore} is BETTER than current {loadedScore}. Updating leaderboard");
                YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
            }
            else
            {
                AddLog($"[SetNewScore] Current score {loadedScore} is BETTER than new {newScore}. No update needed");
            }
        }
        else
        {
            AddLog("[SetNewScore] Failed to retrieve current score, setting as initial");
            YG2.SetLBTimeConvert(_leaderboard.nameLB, newScore);
        }

        _leaderboard.UpdateLB();
        AddLog("[SetNewScore] Leaderboard updated locally");
    }

    private void OnLeaderboardDataReceived(LBData lbData)
    {
        Debug.Log($"lbData = {lbData}");
        AddLog($"[OnLeaderboardDataReceived] Received data for leaderboard: {lbData.technoName}");

        if (lbData.technoName == _leaderboard.nameLB)
        {
            _lbData = lbData;
            AddLog($"[OnLeaderboardDataReceived] Data stored for leaderboard {_leaderboard.nameLB}");
            AddLog($"[OnLeaderboardDataReceived] Entries status: {lbData.entries}");
            AddLog($"[OnLeaderboardDataReceived] Number of players: {lbData.players?.Count() ?? 0}");

            // Если был отложенный результат — отправляем его сейчас
            if (_pendingScore > 0)
            {
                AddLog("[OnLeaderboardDataReceived] Pending score found. Sending now...");
                SetNewScore(_pendingScore);
                _pendingScore = 0; // Очищаем буфер после отправки
            }
        }
        else
        {
            AddLog($"[OnLeaderboardDataReceived] Ignoring data for different leaderboard: {lbData.technoName}");
        }
    }

    public bool TryGetScore(out float score, out bool isEmptyScore)
    {
        isEmptyScore = false;
        score = 0;

        AddLog("[TryGetScore] Starting score retrieval process");

        if (_lbData == null)
        {
            AddLog("[TryGetScore] _lbData is null — no leaderboard data available");
            return false;
        }

        if (_lbData.entries == InfoYG.NO_DATA)
        {
            AddLog("[TryGetScore] Leaderboard has no data (NO_DATA)");
            isEmptyScore = true;
            return false;
        }

        AddLog($"[TryGetScore] Searching among {_lbData.players?.Count() ?? 0} players");

        foreach (var player in _lbData.players)
        {
            AddLog($"[TryGetScore] Checking player: ID={player.uniqueID}, Score={player.score}");

            if (player.uniqueID == YG2.player.id)
            {
                score = player.score / 1000f; // Предполагаем, что score в миллисекундах
                AddLog($"[TryGetScore] Found matching player! Raw score: {player.score}, Converted: {score}");
                return true;
            }
        }

        AddLog($"[TryGetScore] Player ID {YG2.player.id} not found in leaderboard entries");
        return false;
    }
}
