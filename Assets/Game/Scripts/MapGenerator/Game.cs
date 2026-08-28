using System;
using System.Collections;
using Game.Scripts.Lifecycle;
using Game.Scripts.MiniGameCore;
using Game.Scripts.UI;
using Game.Scripts.UI.Menu;
using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Transition _transition;
        [SerializeField] private Transform _canvasTransform;
        [SerializeField] private GameOverMenu _gameOverMenu;
        [SerializeField] private VictoryMenu _victoryMenu;
        [SerializeField] private MainMenu _mainMenu;

        [Header("Transition Colors")] [SerializeField]
        private Material _goodMaterial;

        [SerializeField] private Material _badMaterial;

        [Header("Other settings")] [SerializeField]
        private float _transitionDuration = 0.75f;

        [SerializeField] private MiniGame _miniGame;
        [SerializeField] private DeathModule _deathModule;
        [SerializeField] private GameHeart _gameHeart;

        private Coroutine _currentCoroutine;

        public event Action Started;
        public event Action Continued;
        public event Action Lost;
        public event Action Completed;
        public event Action Restarted;
        public event Action Leaved;
        public event Action Transited;
        
        public bool HasCompleted { get; private set; }
        public bool HasStarted { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            IsPause = false;
            IsPlaying = false;
        }

        private void OnEnable()
        {
            _deathModule.SnakeDied += OnGameComplete;
        }

        private void OnDisable()
        {
            _deathModule.SnakeDied -= OnGameComplete;
        }

        private void OnApplicationQuit()
        {
            if (IsPlaying)
                _gameHeart.DecreaseCount();
        }

        public void Begin()
        {
            if (_gameHeart.IsPossibleDecrease)
                StartCoroutine(BeginRoutine());
        }

        public void Continue()
        {
            StartCoroutine(ContinueRoutine());
        }

        public void Over()
        {
            StartCoroutine(OverRoutine());
        }

        public void Restart()
        {
            StartCoroutine(RestartRoutine());
        }

        public void Leave()
        {
            StartCoroutine(LeaveRoutine());
        }

        public void FastLeave()
        {
            StartCoroutine(FastLeaveRoutine());
        }

        public void StopTime()
        {
            Time.timeScale = 0f;
            IsPause = true;
        }

        public void ContinueTime()
        {
            Time.timeScale = 1f;
            IsPause = false;
        }

        private IEnumerator BeginRoutine()
        {
            _transition.StartTransition(_goodMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Started?.Invoke();

            _transition.ContinueTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            HasStarted = true;
            HasCompleted = false;
            IsPlaying = true;
        }

        private IEnumerator ContinueRoutine()
        {
            Continued?.Invoke();
            _gameHeart.transform.SetParent(_mainMenu.transform);
            _gameHeart.gameObject.SetActive(false);

            _transition.ContinueTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Transited?.Invoke();
            HasCompleted = false;
            IsPlaying = true;
        }

        private IEnumerator CompleteRoutine()
        {
            IsPlaying = false;
            HasCompleted = true;
            Completed?.Invoke();

            _transition.StartBackTransition(_goodMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);
        }

        private IEnumerator LeaveRoutine()
        {
            IsPlaying = false;
            Leaved?.Invoke();

            _gameHeart.transform.SetParent(_mainMenu.transform);
            _gameHeart.gameObject.SetActive(!_miniGame.IsActive);

            _transition.ContinueBackTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Transited?.Invoke();
        }

        private IEnumerator FastLeaveRoutine()
        {
            _transition.StartBackTransition(_badMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            IsPlaying = false;
            HasCompleted = false;
            Leaved?.Invoke();

            _gameHeart.transform.SetParent(_mainMenu.transform);

            _transition.ContinueBackTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            yield return StartCoroutine(_gameHeart.UseHeartRoutine());
        }

        private IEnumerator OverRoutine()
        {
            IsPlaying = false;
            HasCompleted = false;
            Lost?.Invoke();

            _transition.StartBackTransition(_badMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            _gameHeart.transform.SetParent(_gameOverMenu.transform);
            _gameHeart.gameObject.SetActive(true);

            yield return StartCoroutine(_gameHeart.UseHeartRoutine());
        }

        private IEnumerator RestartRoutine()
        {
            Restarted?.Invoke();
            _gameHeart.transform.SetParent(_mainMenu.transform);
            _gameHeart.gameObject.SetActive(false);

            _transition.ContinueTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Transited?.Invoke();
            IsPlaying = true;
        }

        private void OnGameComplete()
        {
            StartCoroutine(CompleteRoutine());
        }
    }
}