using System;
using UnityEngine;
using YG;

namespace Game.Scripts.UI.Menu
{
    public class HeartTimer
    {
        private const int MAX_HEARTS = 3;
        private const int RESTORE_TIME_SECONDS = 900;

        public event Action HeartsChanged;

        private DateTime? _nextRestoreTimeUtc;
        private int _pendingRestores;
        private bool _isRestoring;

        public bool IsInitialized { get; private set; }
        public int CurrentHearts { get; private set; }

        public int MaxHearts => MAX_HEARTS;
        public bool HasAvailableHearts => CurrentHearts > 0;

        public void Initialize()
        {
            if (IsInitialized) return;

            CurrentHearts = YG2.saves.HeartCount;
            _pendingRestores = YG2.saves.PendingRestores;

            var restoreTimeString = YG2.saves.NextRestoreTime;

            if (!string.IsNullOrEmpty(restoreTimeString))
            {
                if (DateTime.TryParse(restoreTimeString, null, 
                        System.Globalization.DateTimeStyles.RoundtripKind, out var parsedTime))
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
            HeartsChanged?.Invoke();
        }

        public string GetTimerText()
        {
            if (CurrentHearts >= MAX_HEARTS || !_isRestoring || _nextRestoreTimeUtc == null)
            {
                return string.Empty;
            }

            var timeRemaining = _nextRestoreTimeUtc.Value - DateTime.UtcNow;

            if (timeRemaining <= TimeSpan.Zero)
            {
                return "00:00";
            }

            return timeRemaining.TotalHours >= 1 ? $@"{timeRemaining:h\:mm\:ss}" : $"{timeRemaining:mm\\:ss}";
        }

        public float GetFillAmount()
        {
            return (float)CurrentHearts / MAX_HEARTS;
        }

        public void UpdateTimer()
        {
            if (!_isRestoring || _nextRestoreTimeUtc == null) return;

            var nowUtc = DateTime.UtcNow;

            if (nowUtc >= _nextRestoreTimeUtc.Value)
            {
                CompleteRestore();
            }
        }

        public bool TryUseHeart()
        {
            if (CurrentHearts <= 0) return false;

            CurrentHearts--;
            _pendingRestores++;

            if (!_isRestoring)
            {
                StartNextRestore();
            }

            SaveData();
            HeartsChanged?.Invoke();
            return true;
        }

        public void SetCurrentHearts(int newCount)
        {
            CurrentHearts = Mathf.Clamp(newCount, 0, MAX_HEARTS);
            SaveData();
            HeartsChanged?.Invoke();
        }

        private void ValidateData()
        {
            CurrentHearts = Mathf.Clamp(CurrentHearts, 0, MAX_HEARTS);

            if (CurrentHearts > MAX_HEARTS)
            {
                CurrentHearts = MAX_HEARTS;
                _pendingRestores = 0;
                _isRestoring = false;
                _nextRestoreTimeUtc = null;
            }

            if (CurrentHearts == MAX_HEARTS)
            {
                _pendingRestores = 0;
                _isRestoring = false;
                _nextRestoreTimeUtc = null;
            }

            var maxPending = MAX_HEARTS - CurrentHearts;

            if (_pendingRestores > maxPending)
            {
                _pendingRestores = maxPending;
            }   
        }

        private void ProcessOfflineRestores()
        {
            if (!_nextRestoreTimeUtc.HasValue || _pendingRestores <= 0 || CurrentHearts >= MAX_HEARTS)
            {
                return;
            }

            var nowUtc = DateTime.UtcNow;

            if (nowUtc < _nextRestoreTimeUtc.Value)
            {
                _isRestoring = true;
                return;
            }

            var timePassed = nowUtc - _nextRestoreTimeUtc.Value;

            if (timePassed.TotalSeconds < RESTORE_TIME_SECONDS)
            {
                CompleteSingleRestore();

                if (_pendingRestores > 0 && CurrentHearts < MAX_HEARTS)
                {
                    StartNextRestore();
                }
                return;
            }

            var fullRestores = (int)(timePassed.TotalSeconds / RESTORE_TIME_SECONDS);
            var heartsToAdd = Mathf.Min(fullRestores, _pendingRestores);

            if (heartsToAdd > 0)
            {
                CurrentHearts += heartsToAdd;
                _pendingRestores -= heartsToAdd;

                if (CurrentHearts > MAX_HEARTS) CurrentHearts = MAX_HEARTS;
                if (_pendingRestores < 0) _pendingRestores = 0;

                var remainingSeconds = timePassed.TotalSeconds % RESTORE_TIME_SECONDS;

                if (_pendingRestores > 0 && CurrentHearts < MAX_HEARTS)
                {
                    _nextRestoreTimeUtc = nowUtc.AddSeconds(RESTORE_TIME_SECONDS - remainingSeconds);
                    _isRestoring = true;
                }
                else
                {
                    _nextRestoreTimeUtc = null;
                    _isRestoring = false;
                }

                HeartsChanged?.Invoke();
            }
            else
            {
                var remainingSeconds = timePassed.TotalSeconds % RESTORE_TIME_SECONDS;
                _nextRestoreTimeUtc = nowUtc.AddSeconds(RESTORE_TIME_SECONDS - remainingSeconds);
                _isRestoring = true;
            }
        }

        private void CompleteSingleRestore()
        {
            if (_pendingRestores <= 0 || CurrentHearts >= MAX_HEARTS) return;

            CurrentHearts++;
            _pendingRestores--;

            if (CurrentHearts > MAX_HEARTS) CurrentHearts = MAX_HEARTS;
            if (_pendingRestores < 0) _pendingRestores = 0;
        }

        private void StartNextRestore()
        {
            if (_pendingRestores <= 0 || CurrentHearts >= MAX_HEARTS)
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
            if (_pendingRestores <= 0 || CurrentHearts >= MAX_HEARTS) return;

            CurrentHearts++;
            _pendingRestores--;

            if (_pendingRestores > 0 && CurrentHearts < MAX_HEARTS)
            {
                StartNextRestore();
            }
            else
            {
                _isRestoring = false;
                _nextRestoreTimeUtc = null;
            }

            SaveData();
            HeartsChanged?.Invoke();
        }

        private void SaveData()
        {
            YG2.saves.HeartCount = CurrentHearts;
            YG2.saves.PendingRestores = _pendingRestores;

            if (_nextRestoreTimeUtc.HasValue)
            {
                var utcString = _nextRestoreTimeUtc.Value.ToString("o");
                YG2.saves.NextRestoreTime = utcString;
            }
            else
            {
                YG2.saves.NextRestoreTime = string.Empty;
            }

            YG2.SaveProgress();
        }
    }
}