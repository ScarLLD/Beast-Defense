using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesInteractor : MonoBehaviour
{
    [SerializeField] private RayCreator _ray;
    [SerializeField] private ShootingPlacesHolder _placesHolder;

    private void OnEnable()
    {
        _ray.Clicked += TryGetMove;
    }

    private void OnDisable()
    {
        _ray.Clicked -= TryGetMove;
    }

    private void TryGetMove(Cube cube)
    {
        Debug.Log("TryGetMove");

        if (_placesHolder.TryGetPlace(out ShootingPlace place) && cube.IsStatic && cube.IsAvailable)
        {
            Debug.Log("placeGeted");
            cube.ChangeStaticStatus();
            cube.Mover.MoveTarget(place.gameObject.transform);
        }
    }
}
