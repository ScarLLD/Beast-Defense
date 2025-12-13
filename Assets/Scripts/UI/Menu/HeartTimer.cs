using System;
using UnityEngine;

[System.Serializable]
public class HeartTimer
{
    [SerializeField] private int _maxHearts = 5;
    [SerializeField] private float _restoreTimeMinutes = 20f;

    private int _currentHearts;
    private DateTime _nextRestoreTime;
    private bool _isInitialized = false;

    private const string NEXT_RESTORE_KEY = "NextRestoreTime";
    private const string CURRENT_COUNT_KEY = "CurrentHeartCount";

    public int CurrentHearts => _currentHearts;
    public int MaxHearts => _maxHearts;
    public DateTime NextRestoreTime => _nextRestoreTime;
    public float TimeUntilNextRestore => Mathf.Max((float)(_nextRestoreTime - DateTime.Now).TotalMinutes, 0f);
    public bool HasAvailableHearts => _currentHearts > 0;
    public bool IsInitialized => _isInitialized;

    public event Action OnHeartsChanged;
    public event Action OnHeartRestored;

    public void Initialize()
    {
        if (_isInitialized) return;

        LoadSavedData();
        RestoreOfflineHearts();
        _isInitialized = true;
    }

    public bool TryUseHeart()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("HeartTimer не инициализирован!");
            return false;
        }

        if (_currentHearts <= 0) return false;

        _currentHearts--;

        if (_currentHearts < _maxHearts)
        {
            _nextRestoreTime = DateTime.Now.AddMinutes(_restoreTimeMinutes);
        }

        SaveData();
        OnHeartsChanged?.Invoke();
        return true;
    }

    public void RestoreOneHeart()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("HeartTimer не инициализирован!");
            return;
        }

        if (_currentHearts >= _maxHearts) return;

        _currentHearts = Mathf.Min(_currentHearts + 1, _maxHearts);

        if (_currentHearts < _maxHearts)
        {
            _nextRestoreTime = DateTime.Now.AddMinutes(_restoreTimeMinutes);
        }

        SaveData();
        OnHeartsChanged?.Invoke();
        OnHeartRestored?.Invoke();
    }

    public void RestoreAllHearts()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("HeartTimer не инициализирован!");
            return;
        }

        _currentHearts = _maxHearts;
        _nextRestoreTime = DateTime.Now;
        SaveData();
        OnHeartsChanged?.Invoke();
    }

    public string GetTimerText()
    {
        if (!_isInitialized) return "«агрузка...";
        if (_currentHearts >= _maxHearts) return string.Empty;

        TimeSpan timeLeft = _nextRestoreTime - DateTime.Now;

        if (timeLeft.TotalSeconds <= 0)
        {
            return "√отово!";
        }

        return $"{timeLeft.Minutes:00}:{timeLeft.Seconds:00}";
    }

    public bool ShouldRestoreHeart()
    {
        if (!_isInitialized) return false;
        return DateTime.Now >= _nextRestoreTime && _currentHearts < _maxHearts;
    }

    public void ForceRestoreIfNeeded()
    {
        if (!_isInitialized) return;

        if (ShouldRestoreHeart())
        {
            RestoreOneHeart();
        }
    }

    public float GetFillAmount()
    {
        if (!_isInitialized) return 1f;
        return (float)_currentHearts / _maxHearts;
    }

    private void LoadSavedData()
    {
        _currentHearts = PlayerPrefs.GetInt(CURRENT_COUNT_KEY, _maxHearts);

        string savedTime = PlayerPrefs.GetString(NEXT_RESTORE_KEY, "");

        if (!string.IsNullOrEmpty(savedTime) && DateTime.TryParse(savedTime, out DateTime savedDateTime))
        {
            _nextRestoreTime = savedDateTime;
        }
        else
        {
            _nextRestoreTime = DateTime.Now;
        }
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(CURRENT_COUNT_KEY, _currentHearts);
        PlayerPrefs.SetString(NEXT_RESTORE_KEY, _nextRestoreTime.ToString());
        PlayerPrefs.Save();
    }

    private void RestoreOfflineHearts()
    {
        if (_currentHearts >= _maxHearts) return;

        TimeSpan timePassed = DateTime.Now - _nextRestoreTime;
        if (timePassed.TotalMinutes < 0) return;

        int heartsToRestore = Mathf.FloorToInt((float)(timePassed.TotalMinutes / _restoreTimeMinutes));

        if (heartsToRestore > 0)
        {
            _currentHearts = Mathf.Min(_currentHearts + heartsToRestore, _maxHearts);

            double remainingMinutes = timePassed.TotalMinutes % _restoreTimeMinutes;
            _nextRestoreTime = DateTime.Now.AddMinutes(_restoreTimeMinutes - remainingMinutes);

            SaveData();
        }
    }
}