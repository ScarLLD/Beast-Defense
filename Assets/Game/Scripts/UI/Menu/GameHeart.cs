using System;
using System.Collections;
using Game.Scripts.MiniGameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menu
{
    public class GameHeart : MonoBehaviour
    {
        private const float UPDATE_UI_DELAY = 1f;
        private static readonly int Shake = Animator.StringToHash("Shake");

        [Header("UI элементы")] 
        [SerializeField] private TMP_Text _countText;
        [SerializeField] private Image _heartImage;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private AnimationCurve _changeAnimationCurve;

        [Header("Анимации")] 
        [SerializeField] private float _changeDuration = 0.5f;
        [SerializeField] private float _afterAnimateDelay = 0.2f;

        [Header("Другое")] [SerializeField] private Adv _adv;
        [SerializeField] private MiniGame _miniGame;

        private WaitForSecondsRealtime _updateUISleep;
        private WaitForSeconds _afterAnimateSleep;
        private HeartTimer _heartTimer;
        private Animator _animator;
        private Coroutine _timerCoroutine;
        private Coroutine _heartUpdateCoroutine;
        private bool _isAnimating;
        private bool _isAnimatingHeartChange;
        private bool _isFirstUpdate = true;
        private int _lastHeartCount;

        public event Action Devastated;

        public bool IsPossibleDecrease => _heartTimer?.HasAvailableHearts ?? false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _heartTimer = new HeartTimer();
            _heartTimer.HeartsChanged += OnHeartsChanged;

            _updateUISleep = new WaitForSecondsRealtime(UPDATE_UI_DELAY);
            _afterAnimateSleep = new WaitForSeconds(_afterAnimateDelay);

            _isAnimating = false;
            _isAnimatingHeartChange = false;
            _isAnimatingHeartChange = false;
            _lastHeartCount = 0;
        }

        private void Start()
        {
            if (_heartTimer is { IsInitialized: false })
            {
                _heartTimer.Initialize();
            }

            _lastHeartCount = _heartTimer?.CurrentHearts ?? 0;
            UpdateUI();

            StartTimerUpdate();
            StartHeartUpdateCoroutine();
        }

        private void OnEnable()
        {
            UpdateUI();
            StartTimerUpdate();
            StartHeartUpdateCoroutine();

            _adv.HeartIncreased += OnHeartIncreased;
            _miniGame.Won += OnHeartIncreased;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            _timerCoroutine = null;
            _heartUpdateCoroutine = null;

            _adv.HeartIncreased -= OnHeartIncreased;
            _miniGame.Won -= OnHeartIncreased;
        }

        public void PlayShakeAnimation()
        {
            if (_animator)
            {
                _animator.enabled = true;
                _animator.SetTrigger(Shake);
            }

            Devastated?.Invoke();
        }

        public IEnumerator UseHeartRoutine()
        {
            if (_heartTimer is not { IsInitialized: true } || !_heartTimer.HasAvailableHearts || _isAnimating)
                yield break;

            _isAnimating = true;

            var previousCount = _heartTimer.CurrentHearts;

            var success = _heartTimer.TryUseHeart();

            if (!success)
            {
                _isAnimating = false;
                yield break;
            }

            yield return StartCoroutine(AnimateHeartChange(
                previousCount,
                _heartTimer.CurrentHearts,
                _changeAnimationCurve));

            yield return _afterAnimateSleep;

            _isAnimating = false;
        }

        public void DecreaseCount()
        {
            _heartTimer.TryUseHeart();
        }

        private void OnDestroy()
        {
            if (_heartTimer != null)
            {
                _heartTimer.HeartsChanged -= OnHeartsChanged;
            }
        }

        private void OnHeartIncreased()
        {
            if (_heartTimer is not { IsInitialized: true } || _isAnimating || _isAnimatingHeartChange)
                return;

            _isAnimatingHeartChange = true;
            StartCoroutine(RestoreHeartAnimationRoutine(_lastHeartCount, _lastHeartCount + 1));
        }

        private void StartHeartUpdateCoroutine()
        {
            if (_heartUpdateCoroutine != null)
                StopCoroutine(_heartUpdateCoroutine);

            _heartUpdateCoroutine = StartCoroutine(HeartUpdateRoutine());
        }

        private IEnumerator HeartUpdateRoutine()
        {
            while (true)
            {
                if (_heartTimer is { IsInitialized: true })
                {
                    _heartTimer.UpdateTimer();
                }

                yield return null;
            }
        }

        private void OnHeartsChanged()
        {
            if (_heartTimer == null) return;

            var currentCount = _heartTimer.CurrentHearts;

            if (_isFirstUpdate)
            {
                _isFirstUpdate = false;
                _lastHeartCount = currentCount;
                UpdateUI();
                return;
            }

            if (_isAnimatingHeartChange)
            {
                return;
            }

            if (currentCount > _lastHeartCount && !_isAnimating)
            {
                StartCoroutine(RestoreHeartAnimationRoutine(_lastHeartCount, currentCount));
            }
            else
            {
                UpdateUI();
            }

            _lastHeartCount = currentCount;
        }

        private void StartTimerUpdate()
        {
            if (_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerCoroutine = StartCoroutine(TimerUpdateRoutine());
        }

        private IEnumerator TimerUpdateRoutine()
        {
            while (true)
            {
                UpdateTimerText();
                yield return _updateUISleep;
            }
        }

        private IEnumerator RestoreHeartAnimationRoutine(int startCount, int endCount)
        {
            if (endCount > _heartTimer.MaxHearts)
            {
                _isAnimatingHeartChange = false;
                yield break;
            }

            if (_isAnimating)
            {
                while (_isAnimating)
                    yield return null;
            }

            _isAnimating = true;

            yield return StartCoroutine(AnimateHeartChange(
                startCount,
                endCount,
                _changeAnimationCurve));

            yield return new WaitForSecondsRealtime(_afterAnimateDelay);

            _isAnimating = false;
            _isAnimatingHeartChange = false;

            _lastHeartCount = endCount;
            UpdateUI();
        }

        private IEnumerator AnimateHeartChange(int startCount, int endCount, AnimationCurve curve)
        {
            var timer = 0f;
            var startFillAmount = (float)startCount / _heartTimer.MaxHearts;
            var targetFillAmount = (float)endCount / _heartTimer.MaxHearts;

            while (timer < _changeDuration)
            {
                timer += Time.deltaTime;
                var t = timer / _changeDuration;

                _heartImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curve.Evaluate(t));

                var displayCount = Mathf.RoundToInt(Mathf.Lerp(startCount, endCount, t));
                _countText.text = $"{displayCount}/{_heartTimer.MaxHearts}";

                yield return null;
            }

            _heartTimer.SetCurrentHearts(endCount);
            _heartImage.fillAmount = targetFillAmount;
            _countText.text = $"{endCount}/{_heartTimer.MaxHearts}";
        }

        private void UpdateUI()
        {
            if (_heartTimer is not { IsInitialized: true })
            {
                _heartImage.fillAmount = 1f;
                _countText.text = $"{_heartTimer?.CurrentHearts ?? 0}/{_heartTimer?.MaxHearts ?? 5}";

                if (_timerText) _timerText.text = string.Empty;
                return;
            }

            _heartImage.fillAmount = _heartTimer.GetFillAmount();
            _countText.text = $"{_heartTimer.CurrentHearts}/{_heartTimer.MaxHearts}";
            UpdateTimerText();
        }

        private void UpdateTimerText()
        {
            if (_timerText)
            {
                _timerText.text = _heartTimer?.GetTimerText() ?? string.Empty;
            }
        }
    }
}