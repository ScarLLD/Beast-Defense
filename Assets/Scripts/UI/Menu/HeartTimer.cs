using System;
using UnityEngine;

public class HeartTimer
{
    private const int MAX_HEARTS = 3;
    private const int RESTORE_TIME_SECONDS = 900;

    public event Action OnHeartsChanged;

    private int _currentHearts;
    private int _pendingRestores = 0;
    private DateTime? _nextRestoreTimeUtc;
    private bool _isRestoring = false;

    public bool IsInitialized { get; private set; }
    public int CurrentHearts => _currentHearts;
    public int MaxHearts => MAX_HEARTS;
    public bool HasAvailableHearts => _currentHearts > 0;

    public void Initialize()
    {
        if (IsInitialized) return;

        _currentHearts = PlayerPrefs.GetInt("HeartCount", MAX_HEARTS);
        _pendingRestores = PlayerPrefs.GetInt("PendingRestores", 0);

        string restoreTimeString = PlayerPrefs.GetString("NextRestoreTime", "");

        if (!string.IsNullOrEmpty(restoreTimeString))
        {
            if (DateTime.TryParse(restoreTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsedTime))
            {
                _nextRestoreTimeUtc = parsedTime.ToUniversalTime();
            }
            else
            {
                _nextRestoreTimeUtc = null;
            }
        }
        else
        {
            _nextRestoreTimeUtc = null;
        }

        ValidateData();

        ProcessOfflineRestores();

        IsInitialized = true;
        SaveData();
        OnHeartsChanged?.Invoke();
    }

    public string GetTimerText()
    {
        if (_currentHearts >= MAX_HEARTS)
        {
            return string.Empty;
        }

        if (!_isRestoring || _nextRestoreTimeUtc == null)
        {
            return string.Empty;
        }

        TimeSpan timeRemaining = _nextRestoreTimeUtc.Value - DateTime.UtcNow;

        if (timeRemaining <= TimeSpan.Zero)
        {
            return "00:00";
        }

        if (timeRemaining.TotalHours >= 1)
        {
            return $"{timeRemaining:h\\:mm\\:ss}";
        }

        return $"{timeRemaining:mm\\:ss}";
    }

    public float GetFillAmount()
    {
        return (float)_currentHearts / MAX_HEARTS;
    }

    public void UpdateTimer()
    {
        if (!_isRestoring || _nextRestoreTimeUtc == null) return;

        DateTime nowUtc = DateTime.UtcNow;

        if (nowUtc >= _nextRestoreTimeUtc.Value)
        {
            CompleteRestore();
        }
    }

    public bool TryUseHeart()
    {
        if (_currentHearts <= 0) return false;

        _currentHearts--;
        _pendingRestores++;

        if (!_isRestoring)
        {
            StartNextRestore();
        }

        SaveData();
        OnHeartsChanged?.Invoke();
        return true;
    }

    private void ValidateData()
    {
        _currentHearts = Mathf.Clamp(_currentHearts, 0, MAX_HEARTS);

        if (_currentHearts > MAX_HEARTS)
        {
            _currentHearts = MAX_HEARTS;
            _pendingRestores = 0;
            _isRestoring = false;
            _nextRestoreTimeUtc = null;
        }

        if (_currentHearts == MAX_HEARTS)
        {
            _pendingRestores = 0;
            _isRestoring = false;
            _nextRestoreTimeUtc = null;
        }

        int maxPending = MAX_HEARTS - _currentHearts;
        if (_pendingRestores > maxPending)
        {
            _pendingRestores = maxPending;
        }
    }

    private void ProcessOfflineRestores()
    {
        if (!_nextRestoreTimeUtc.HasValue || _pendingRestores <= 0 || _currentHearts >= MAX_HEARTS)
        {
            return;
        }

        DateTime nowUtc = DateTime.UtcNow;

        if (nowUtc < _nextRestoreTimeUtc.Value)
        {
            _isRestoring = true;
            return;
        }

        TimeSpan timePassed = nowUtc - _nextRestoreTimeUtc.Value;

        if (timePassed.TotalSeconds < RESTORE_TIME_SECONDS)
        {
            CompleteSingleRestore();

            if (_pendingRestores > 0 && _currentHearts < MAX_HEARTS)
            {
                StartNextRestore();
            }
            return;
        }

        int fullRestores = (int)(timePassed.TotalSeconds / RESTORE_TIME_SECONDS);
        int heartsToAdd = Mathf.Min(fullRestores, _pendingRestores);

        if (heartsToAdd > 0)
        {
            _currentHearts += heartsToAdd;
            _pendingRestores -= heartsToAdd;

            if (_currentHearts > MAX_HEARTS) _currentHearts = MAX_HEARTS;
            if (_pendingRestores < 0) _pendingRestores = 0;

            double remainingSeconds = timePassed.TotalSeconds % RESTORE_TIME_SECONDS;

            if (_pendingRestores > 0 && _currentHearts < MAX_HEARTS)
            {
                _nextRestoreTimeUtc = nowUtc.AddSeconds(RESTORE_TIME_SECONDS - remainingSeconds);
                _isRestoring = true;
            }
            else
            {
                _nextRestoreTimeUtc = null;
                _isRestoring = false;
            }

            OnHeartsChanged?.Invoke();
        }
        else
        {
            double remainingSeconds = timePassed.TotalSeconds % RESTORE_TIME_SECONDS;
            _nextRestoreTimeUtc = nowUtc.AddSeconds(RESTORE_TIME_SECONDS - remainingSeconds);
            _isRestoring = true;
        }
    }

    private void CompleteSingleRestore()
    {
        if (_pendingRestores <= 0 || _currentHearts >= MAX_HEARTS) return;

        _currentHearts++;
        _pendingRestores--;

        if (_currentHearts > MAX_HEARTS) _currentHearts = MAX_HEARTS;
        if (_pendingRestores < 0) _pendingRestores = 0;
    }
    
    private void StartNextRestore()
    {
        if (_pendingRestores <= 0 || _currentHearts >= MAX_HEARTS)
        {
            _isRestoring = false;
            _nextRestoreTimeUtc = null;
            SaveData();
            return;
        }

        _isRestoring = true;
        _nextRestoreTimeUtc = DateTime.UtcNow.AddSeconds(RESTORE_TIME_SECONDS);

        SaveData();
    }

    private void CompleteRestore()
    {
        if (_pendingRestores <= 0 || _currentHearts >= MAX_HEARTS) return;

        _currentHearts++;
        _pendingRestores--;

        if (_pendingRestores > 0 && _currentHearts < MAX_HEARTS)
        {
            StartNextRestore();
        }
        else
        {
            _isRestoring = false;
            _nextRestoreTimeUtc = null;
        }

        SaveData();
        OnHeartsChanged?.Invoke();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("HeartCount", _currentHearts);
        PlayerPrefs.SetInt("PendingRestores", _pendingRestores);

        if (_nextRestoreTimeUtc.HasValue)
        {
            string utcString = _nextRestoreTimeUtc.Value.ToString("o");
            PlayerPrefs.SetString("NextRestoreTime", utcString);
        }
        else
        {
            PlayerPrefs.SetString("NextRestoreTime", "");
        }

        PlayerPrefs.Save();
    }
}