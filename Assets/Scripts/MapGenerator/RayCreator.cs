using System;
using System.Collections;
using UnityEngine;

public class RayCreator : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Game _game;
    [SerializeField] private float _rayDirection;

    private WaitForSeconds _sleepTime;
    private WaitForSeconds _clickCooldown;
    private Coroutine _rayCoroutine;
    private bool _isClickProcessed;

    public event Action<PlayerCube> Clicked;

    private void Awake()
    {
        _sleepTime = new WaitForSeconds(0.01f);
        _clickCooldown = new WaitForSeconds(0.1f);
    }

    private void OnEnable()
    {
        _game.Started += EnableRay;
        _game.Restarted += EnableRay;
        _game.Continued += EnableRay;
        _game.Completed += DisableRay;
        _game.Loss += DisableRay;
    }

    private void OnDisable()
    {
        _game.Started -= EnableRay;
        _game.Restarted -= EnableRay;
        _game.Continued -= EnableRay;
        _game.Completed -= DisableRay;
        _game.Loss -= DisableRay;
    }

    private void EnableRay()
    {
        DisableRay();
        _rayCoroutine ??= StartCoroutine(MouseRaycastInteraction());
    }

    private void DisableRay()
    {
        if (_rayCoroutine != null)
        {
            StopCoroutine(_rayCoroutine);
            _rayCoroutine = null;
        }

        _isClickProcessed = false;
    }

    private IEnumerator MouseRaycastInteraction()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_game.IsPause == false && _game.IsPlaying == true)
            {
                Ray ray = new();
                bool hasInput = false;

                if (Input.GetMouseButtonDown(0))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    hasInput = true;
                }
                else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    hasInput = true;
                }

                if (hasInput && !_isClickProcessed && Physics.Raycast(ray, out RaycastHit hit, _rayDirection))
                {
                    if (hit.transform.TryGetComponent(out PlayerCube cube) && cube.IsAvailable && cube.IsScaling == false)
                    {
                        _isClickProcessed = true;
                        Clicked?.Invoke(cube);
                        yield return StartCoroutine(ResetClickCooldown());
                    }
                }
            }

            yield return _sleepTime;
        }
    }

    private IEnumerator ResetClickCooldown()
    {
        yield return _clickCooldown;
        _isClickProcessed = false;
    }
}