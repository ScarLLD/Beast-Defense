using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHeart : MonoBehaviour
{
    [Header("UI элементы")]
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Image _heartImage;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private AnimationCurve _changeAnimationCurve;

    [Header("Анимации")]
    [SerializeField] private float _changeDuration = 0.5f;
    [SerializeField] private float _changeDelay = 0.2f;

    private HeartTimer _heartTimer;
    private Animator _animator;
    private Coroutine _timerCoroutine;
    private Coroutine _heartUpdateCoroutine;
    private bool _isAnimating = false;
    private int _lastHeartCount = 0;
    private bool _isFirstUpdate = true;

    public bool IsPossibleDecrease => _heartTimer?.HasAvailableHearts ?? false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _heartTimer = new HeartTimer();
        _heartTimer.OnHeartsChanged += OnHeartsChanged;
    }

    private void Start()
    {
        if (_heartTimer != null && !_heartTimer.IsInitialized)
        {
            _heartTimer.Initialize();
        }

        _lastHeartCount = _heartTimer?.CurrentHearts ?? 0;
        UpdateUI();

        // Запускаем все корутины
        StartTimerUpdate();
        StartHeartUpdateCoroutine();
    }

    private void OnEnable()
    {
        UpdateUI();
        StartTimerUpdate();
        StartHeartUpdateCoroutine();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _timerCoroutine = null;
        _heartUpdateCoroutine = null;
    }

    private void OnDestroy()
    {
        if (_heartTimer != null)
        {
            _heartTimer.OnHeartsChanged -= OnHeartsChanged;
        }
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
            if (_heartTimer != null && _heartTimer.IsInitialized)
            {
                _heartTimer.UpdateTimer();
            }
            yield return null; // Проверяем каждый кадр
        }
    }

    private void OnHeartsChanged()
    {
        if (_heartTimer == null) return;

        int currentCount = _heartTimer.CurrentHearts;

        // Пропускаем первое обновление при запуске
        if (_isFirstUpdate)
        {
            _isFirstUpdate = false;
            _lastHeartCount = currentCount;
            UpdateUI();
            return;
        }

        // Определяем, было ли восстановление сердца
        if (currentCount > _lastHeartCount && !_isAnimating)
        {
            // Это восстановление в реальном времени
            StartCoroutine(RestoreHeartAnimationRoutine(_lastHeartCount, currentCount));
        }
        else if (currentCount < _lastHeartCount)
        {
            // Это трата сердца (обрабатывается в UseHeartRoutine)
            UpdateUI();
        }
        else
        {
            // Без изменений или другие случаи
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
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator UseHeartRoutine()
    {
        if (_heartTimer == null || !_heartTimer.IsInitialized || !_heartTimer.HasAvailableHearts || _isAnimating)
            yield break;

        _isAnimating = true;

        int previousCount = _heartTimer.CurrentHearts;

        bool success = _heartTimer.TryUseHeart();
        if (!success)
        {
            _isAnimating = false;
            yield break;
        }

        yield return StartCoroutine(AnimateHeartChange(
            previousCount,
            _heartTimer.CurrentHearts,
            _changeAnimationCurve
        ));

        yield return new WaitForSeconds(_changeDelay);

        _isAnimating = false;
    }

    private IEnumerator RestoreHeartAnimationRoutine(int startCount, int endCount)
    {
        if (_isAnimating) yield break;

        _isAnimating = true;

        yield return StartCoroutine(AnimateHeartChange(
            startCount,
            endCount,
            _changeAnimationCurve
        ));

        yield return new WaitForSeconds(_changeDelay);

        _isAnimating = false;

        // Обновляем UI после анимации
        UpdateUI();
    }

    private IEnumerator AnimateHeartChange(int startCount, int endCount, AnimationCurve curve)
    {
        float timer = 0f;
        float startFillAmount = (float)startCount / _heartTimer.MaxHearts;
        float targetFillAmount = (float)endCount / _heartTimer.MaxHearts;

        while (timer < _changeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _changeDuration;

            _heartImage.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curve.Evaluate(t));

            // Обновляем счетчик во время анимации для плавного перехода
            int displayCount = Mathf.RoundToInt(Mathf.Lerp(startCount, endCount, t));
            _countText.text = $"{displayCount}/{_heartTimer.MaxHearts}";

            yield return null;
        }

        _heartImage.fillAmount = targetFillAmount;
        _countText.text = $"{endCount}/{_heartTimer.MaxHearts}";
    }

    public void PlayShakeAnimation()
    {
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.SetTrigger("Shake");
        }
    }

    private void UpdateUI()
    {
        if (_heartTimer == null || !_heartTimer.IsInitialized)
        {
            _heartImage.fillAmount = 1f;
            _countText.text = $"{_heartTimer?.CurrentHearts ?? 0}/{_heartTimer?.MaxHearts ?? 5}";
            if (_timerText != null) _timerText.text = "";
            return;
        }

        _heartImage.fillAmount = _heartTimer.GetFillAmount();
        _countText.text = $"{_heartTimer.CurrentHearts}/{_heartTimer.MaxHearts}";
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            _timerText.text = _heartTimer?.GetTimerText() ?? "";
        }
    }
}