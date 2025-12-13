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
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private AnimationCurve _restoreAnimationCurve;

    [Header("Восстановление")]
    [SerializeField] private GameObject _restorePanel;
    [SerializeField] private Button _restoreButton;

    [Header("Анимации")]
    [SerializeField] private float _changeDuration = 0.5f;
    [SerializeField] private float _changeDelay = 0.2f;

    private HeartTimer _heartTimer;
    private Animator _animator;
    private Coroutine _timerCoroutine;
    private bool _isAnimating = false;

    public bool IsPossibleDecrease => _heartTimer?.HasAvailableHearts ?? false;    

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _heartTimer = new HeartTimer();

        _heartTimer.OnHeartsChanged += OnHeartsChanged;

        if (_restoreButton != null)
        {
            _restoreButton.onClick.AddListener(QuickRestore);
        }
    }

    private void Start()
    {
        if (_heartTimer != null && !_heartTimer.IsInitialized)
        {
            _heartTimer.Initialize();
        }

        UpdateUI();
        StartTimerUpdate();
    }

    private void OnEnable()
    {
        UpdateUI();
        StartTimerUpdate();
    }

    private void OnDisable()
    {
        StopTimerUpdate();
    }

    private void OnDestroy()
    {
        if (_heartTimer != null)
        {
            _heartTimer.OnHeartsChanged -= OnHeartsChanged;
        }
    }

    private void Update()
    {
        if (_heartTimer == null || !_heartTimer.IsInitialized) return;

        if (_heartTimer.ShouldRestoreHeart() && !_isAnimating)
        {
            StartCoroutine(RestoreOneHeartRoutine());
        }
    }

    private void OnHeartsChanged()
    {
        UpdateUI();
    }

    private void StartTimerUpdate()
    {
        if (_timerCoroutine != null)
            StopCoroutine(_timerCoroutine);

        _timerCoroutine = StartCoroutine(TimerUpdateRoutine());
    }

    private void StopTimerUpdate()
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
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
            _deathAnimationCurve
        ));

        yield return new WaitForSeconds(_changeDelay);

        _isAnimating = false;
    }

    private IEnumerator RestoreOneHeartRoutine()
    {
        if (_isAnimating || _heartTimer == null || !_heartTimer.IsInitialized)
            yield break;

        _isAnimating = true;

        int previousCount = _heartTimer.CurrentHearts;
        _heartTimer.RestoreOneHeart();
               
        yield return StartCoroutine(AnimateHeartChange(
            previousCount,
            _heartTimer.CurrentHearts,
            _restoreAnimationCurve
        ));

        _isAnimating = false;
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

            yield return null;
        }

        _heartImage.fillAmount = targetFillAmount;
        _countText.text = $"{_heartTimer.CurrentHearts}/{_heartTimer.MaxHearts}";
    }

    public void QuickRestore()
    {
        if (_heartTimer == null || !_heartTimer.IsInitialized ||
            _heartTimer.CurrentHearts >= _heartTimer.MaxHearts || _isAnimating)
            return;

        int previousCount = _heartTimer.CurrentHearts;
        _heartTimer.RestoreOneHeart();

        StartCoroutine(AnimateHeartChange(
            previousCount,
            _heartTimer.CurrentHearts,
            _restoreAnimationCurve
        ));
    }
        
    public void RestoreAllHearts()
    {
        if (_isAnimating || _heartTimer == null || !_heartTimer.IsInitialized) return;

        _heartTimer.RestoreAllHearts();
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
            _countText.text = $"{_heartTimer.CurrentHearts}/{_heartTimer.MaxHearts}";
            if (_timerText != null) _timerText.text = "Загрузка...";
            if (_restorePanel != null) _restorePanel.SetActive(false);
            return;
        }

        _heartImage.fillAmount = _heartTimer.GetFillAmount();
        _countText.text = $"{_heartTimer.CurrentHearts}/{_heartTimer.MaxHearts}";
        UpdateTimerText();
        UpdateRestorePanel();
    }

    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            _timerText.text = _heartTimer?.GetTimerText() ?? "Загрузка...";
        }
    }

    private void UpdateRestorePanel()
    {
        if (_restorePanel != null && _heartTimer != null && _heartTimer.IsInitialized)
        {
            _restorePanel.SetActive(_heartTimer.CurrentHearts < _heartTimer.MaxHearts);
        }
    }
}