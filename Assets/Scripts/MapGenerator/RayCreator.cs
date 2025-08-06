using System;
using System.Collections;
using UnityEngine;

public class RayCreator : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rayDirection;

    private Coroutine _rayCoroutine;

    public event Action<PlayerCube> Clicked;

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
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetKeyUp(KeyCode.Mouse0)
                && Physics.Raycast(ray.origin, ray.direction,
                out RaycastHit hit, _rayDirection)
                && hit.transform.TryGetComponent(out PlayerCube cube)
                && cube.IsAvailable)
            {
                Clicked?.Invoke(cube);
            }

            yield return null;
        }
    }
}