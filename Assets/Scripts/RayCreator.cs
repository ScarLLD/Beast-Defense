using System;
using UnityEngine;

public class RayCreator : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rayDirection;

    public event Action<Cube> Clicked;

    private void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyUp(KeyCode.Mouse0)
            && Physics.Raycast(ray.origin, ray.direction,
            out RaycastHit hit, _rayDirection)
            && hit.transform.TryGetComponent(out Cube cube)
            && cube.IsAvailable)
        {
            cube.ChangeAvailableStatus();
            Clicked?.Invoke(cube);
        }
    }
}