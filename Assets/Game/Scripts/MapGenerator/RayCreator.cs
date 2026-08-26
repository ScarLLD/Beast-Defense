using Game.Scripts.UI.Menu;
using Game.Scripts.Player;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class RayCreator : MonoBehaviour
    {
        [SerializeField] private PauseMenu _pauseMenu;
        [SerializeField] private Game _game;
        [SerializeField] private float _rayDirection;

        private WaitForSeconds _sleepTime;
        private WaitForSeconds _clickCooldown;
        private Coroutine _rayCoroutine;
        private bool _isClickProcessed;
        private bool _shouldStop;
        private Camera _camera;

        public event Action<PlayerCube> Clicked;

        private void Awake()
        {
            _camera = Camera.main;
            _sleepTime = new WaitForSeconds(0.01f);
            _clickCooldown = new WaitForSeconds(0.1f);
        }

        private void OnEnable()
        {
            _game.Started += EnableRay;
            _game.Restarted += EnableRay;
            _game.Continued += EnableRay;
            _game.Completed += DisableRay;
            _game.Lost += DisableRay;
        }

        private void OnDisable()
        {
            _game.Started -= EnableRay;
            _game.Restarted -= EnableRay;
            _game.Continued -= EnableRay;
            _game.Completed -= DisableRay;
            _game.Lost -= DisableRay;
        }

        private void EnableRay()
        {
            DisableRay();
            _shouldStop = false;
            _rayCoroutine ??= StartCoroutine(MouseRaycastInteraction());
        }

        private void DisableRay()
        {
            _shouldStop = true;

            if (_rayCoroutine != null)
            {
                StopCoroutine(_rayCoroutine);
                _rayCoroutine = null;
            }

            _isClickProcessed = false;
        }

        private IEnumerator MouseRaycastInteraction()
        {
            var isWork = true;

            while (isWork && !_shouldStop)
            {
                isWork = !_shouldStop;

                if (!_game.IsPause && _game.IsPlaying)
                {
                    var hasInput = false;
                    var pos = Vector3.zero;

                    if (Input.GetMouseButtonDown(0))
                    {
                        pos = Input.mousePosition;
                        hasInput = true;
                    }
                    else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        pos = Input.GetTouch(0).position;
                        hasInput = true;
                    }

                    if (hasInput && !_isClickProcessed)
                    {
                        var ray = _camera.ScreenPointToRay(pos);
                        if (Physics.Raycast(ray, out var hit, _rayDirection))
                        {
                            if (hit.transform.TryGetComponent(out PlayerCube cube) &&
                                cube.IsAvailable &&
                                !cube.IsScaling)
                            {
                                _isClickProcessed = true;
                                Clicked?.Invoke(cube);

                                yield return _clickCooldown;
                                _isClickProcessed = false;
                            }
                        }
                    }
                }

                yield return _sleepTime;
            }

            _rayCoroutine = null;
        }
    }
}