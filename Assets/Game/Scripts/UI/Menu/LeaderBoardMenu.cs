using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Menu
{
    public class LeaderBoardMenu : Window
    {
        [SerializeField] private Transition _transition;
        [SerializeField] private Material _leaderBoardMaterial;
        [SerializeField] private float _transitionDuration = 0.4f;
        [SerializeField] private Button _exitButton;

        public event Action Opened;
        public event Action Closed;

        private void Awake()
        {
            DisableMenu();
        }

        private void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void OnDisable()
        {
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        public void Open()
        {
            if (!_transition.IsTransiting)
                StartCoroutine(OpenLeaderBoardRoutine());
        }

        private IEnumerator OpenLeaderBoardRoutine()
        {
            _transition.StartTransition(_leaderBoardMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            EnableMenu();
            Opened?.Invoke();

            _transition.ContinueTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);
        }

        private void OnExitButtonClick()
        {
            CallClickEvent();

            if (!_transition.IsTransiting)
                StartCoroutine(CloseLeaderBoardRoutine());
        }

        private IEnumerator CloseLeaderBoardRoutine()
        {
            if (_transition.IsTransiting) yield break;
            
            _transition.StartBackTransition(_leaderBoardMaterial.color, _transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);

            Closed?.Invoke();
            DisableMenu();

            _transition.ContinueBackTransition(_transitionDuration);
            yield return new WaitUntil(() => !_transition.IsTransiting);
        }
    }
}
