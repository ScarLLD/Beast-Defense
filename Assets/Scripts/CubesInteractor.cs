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
        if (_placesHolder.TryGetPlace(out ShootingPlace place) && cube.IsStatic)
        {
            cube.ChangeStaticStatus();
            cube.Mover.StartMoving(place.gameObject.transform);
        }
    }
}
