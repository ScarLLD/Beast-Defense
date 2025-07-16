using UnityEngine;

public class CubesInteractor : MonoBehaviour
{
    [SerializeField] private RayCreator _ray;
    [SerializeField] private PlaceStorage _placesHolder;
    [SerializeField] private AvailabilityManagement _availabilityManagement;

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
            Vector3 place = new Vector3(shootingPlace.transform.position.x, shootingPlace.transform.position.y + cube.transform.localScale.y / 2, shootingPlace.transform.position.z);
            cube.ChangeStaticStatus(false);
            cube.ChangeAvailableStatus(false);
            cube.Mover.StartMoving(place);
            cube.Mover.SetPlaces(shootingPlace, escapePlace);
            _availabilityManagement.UpdateAvailability();
        }
    }
}
