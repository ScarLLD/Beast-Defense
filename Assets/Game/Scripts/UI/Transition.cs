using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class Transition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private float _holdTime;
        [SerializeField] private Transform _sprite;
        [SerializeField] private Image _spriteImage;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private Canvas _canvas;

        private Vector3 _leftPos;
        private Vector3 _rightPos;

        private Tween _currentTween;
        private bool _isTransiting;

        public bool IsTransiting => _isTransiting;
        public event Action Transiting;
        public event Action BackTransited;

        private void Awake()
        {
            SetSpriteOptions();
            _isTransiting = false;
        }

        private void SetSpriteOptions()
        {            
            float offset = Camera.main.pixelWidth * 3f;
            Vector3 center = _canvas.transform.position;

            _leftPos = new Vector3(center.x - offset, center.y, center.z);
            _rightPos = new Vector3(center.x + offset, center.y, center.z);

            _sprite.localPosition = _leftPos;
            _spriteImage.enabled = false;
            _loadingText.enabled = true;
        }

        public void StartTransition(Color color, float duration)
        {
            if (_isTransiting) return;

            _spriteImage.color = color;
            _spriteImage.enabled = true;
            _loadingText.enabled = true;

            _isTransiting = true;
            Transiting?.Invoke();

            KillCurrentTween();
            _currentTween = _sprite.DOMoveX(_canvas.transform.position.x, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _isTransiting = false;
                });
        }

        public void StartBackTransition(Color color, float duration)
        {
            if (_isTransiting) return;

            _spriteImage.color = color;
            _spriteImage.enabled = true;
            _loadingText.enabled = false;

            _isTransiting = true;
            Transiting?.Invoke();

            KillCurrentTween();
            _currentTween = _sprite.DOMoveX(_canvas.transform.position.x, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _isTransiting = false;
                    BackTransited?.Invoke();
                });
        }
        
        public void ContinueTransition(float duration)
        {
            if (_isTransiting) return;

            _isTransiting = true;
            Transiting?.Invoke();

            // Сначала ждём holdTime, потом двигаем
            StartCoroutine(HoldAndMove(_rightPos, duration));
        }

        public void ContinueBackTransition(float duration)
        {
            if (_isTransiting) return;

            _isTransiting = true;
            Transiting?.Invoke();

            StartCoroutine(HoldAndMove(_leftPos, duration));
        }

        private IEnumerator HoldAndMove(Vector3 targetPosition, float duration)
        {
            yield return new WaitForSeconds(_holdTime);

            KillCurrentTween();
            _currentTween = _sprite.DOMoveX(targetPosition.x, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _isTransiting = false;
                    if (targetPosition == _rightPos)
                        _loadingText.enabled = true;
                    else
                        _spriteImage.enabled = false;
                });
        }

        private void KillCurrentTween()
        {
            if (_currentTween != null && _currentTween.IsActive())
                _currentTween.Kill();
        }
    }
}
