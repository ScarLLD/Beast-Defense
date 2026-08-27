using System;
using System.Collections;
using DG.Tweening;
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

        public bool IsTransiting { get; private set; }

        public event Action Transiting;

        private void Awake()
        {
            SetSpriteOptions();
            IsTransiting = false;
        }

        private void SetSpriteOptions()
        {
            if (Camera.main)
            {
                var offset = Camera.main.pixelWidth * 4f;
                var center = _canvas.transform.position;

                _leftPos = new Vector3(center.x - offset, 0, 0);
                _rightPos = new Vector3(center.x + offset, 0, 0);
            }

            _sprite.localPosition = _leftPos;
            _spriteImage.enabled = false;
            _loadingText.enabled = true;
        }

        public void StartTransition(Color color, float duration)
        {
            if (IsTransiting) return;

            _spriteImage.color = color;
            _spriteImage.enabled = true;
            _loadingText.enabled = true;

            IsTransiting = true;
            Transiting?.Invoke();

            KillCurrentTween();
            _currentTween = _sprite.DOMoveX(_canvas.transform.position.x, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsTransiting = false;
                });
        }

        public void StartBackTransition(Color color, float duration)
        {
            if (IsTransiting) return;

            _spriteImage.color = color;
            _spriteImage.enabled = true;
            _loadingText.enabled = false;

            IsTransiting = true;
            Transiting?.Invoke();

            KillCurrentTween();
            _currentTween = _sprite.DOMoveX(_canvas.transform.position.x, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    IsTransiting = false;
                });
        }

        public void ContinueTransition(float duration)
        {
            if (IsTransiting) return;

            IsTransiting = true;
            Transiting?.Invoke();

            StartCoroutine(HoldAndMove(_rightPos, duration));
        }

        public void ContinueBackTransition(float duration)
        {
            if (IsTransiting) return;

            IsTransiting = true;
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
                    IsTransiting = false;
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
