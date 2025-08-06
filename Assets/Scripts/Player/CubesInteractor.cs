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
        if (_placesHolder.TryGetPlace(out ShootingPlace shootingPlace, out Vector3 escapePlace))
        {
            cube.ChangeStaticStatus(false);
            cube.ChangeAvailableStatus(false);
            cube.Mover.SetPlaces(shootingPlace, escapePlace, cube.GridCell);
            cube.Mover.StartMoving();

            _availabilityManagement.UpdateAvailability();
        }
        else
        {
            Debug.Log("Нет доступных мест для стрельбы.");
        }
    }
}
