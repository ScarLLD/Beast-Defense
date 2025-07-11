using UnityEngine;

public class CubesInteractor : MonoBehaviour
{
    [SerializeField] private RayCreator _ray;
    [SerializeField] private PlaceStorage _placesHolder;

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
        if (_placesHolder.TryGetPlace(out ShootingPlace shootingPlace, out Vector3 escapePlace) && cube.IsStatic)
        {
            cube.ChangeStaticStatus(false);
            cube.ChangeAvailableStatus(false);
            cube.Mover.StartMoving(shootingPlace.transform.position);
            cube.Mover.SetPlaces(shootingPlace, escapePlace);
        }
    }
}
