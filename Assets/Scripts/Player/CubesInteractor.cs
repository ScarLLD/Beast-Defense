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

    private void TryGetMove(PlayerCube cube)
    {
        if (_placesHolder.TryGetPlace(out ShootingPlace place) && cube.IsStatic)
        {
            cube.ChangeStaticStatus(false);
            cube.Mover.StartMoving(place.transform.position);
        }
    }
}
