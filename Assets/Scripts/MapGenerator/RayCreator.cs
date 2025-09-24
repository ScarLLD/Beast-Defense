using System;
using System.Collections;
using UnityEngine;

public class RayCreator : MonoBehaviour
{
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private Game _game;
    [SerializeField] private float _rayDirection;

    private WaitForSeconds _sleepTime;
    private Coroutine _rayCoroutine;

    public event Action<PlayerCube> Clicked;

    private void Awake()
    {
        _sleepTime = new WaitForSeconds(0.1f);
    }

    private void OnEnable()
    {
        _game.Started += ActivateRay;
    }

    private void OnDisable()
    {
        _game.Started -= ActivateRay;
    }

    private void ActivateRay()
    {
        _rayCoroutine ??= StartCoroutine(MouseRaycastInteraction());
    }

    private IEnumerator MouseRaycastInteraction()
    {
        bool isWork = true;

        while (isWork)
        {
            if (_game.IsPause == false)
            {
                Ray ray = new();
                bool hasInput = false;

                if (Input.GetMouseButton(0))
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    hasInput = true;
                }
                else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    hasInput = true;
                }

                if (hasInput && Physics.Raycast(ray, out RaycastHit hit, _rayDirection))
                {
                    if (hit.transform.TryGetComponent(out PlayerCube cube) && cube.IsAvailable)
                    {
                        Clicked?.Invoke(cube);
                    }
                }
            }

            yield return _sleepTime;
        }
    }
}