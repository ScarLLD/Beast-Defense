using UnityEngine;

public class CubesInteractor : MonoBehaviour
{
    [SerializeField] private RayCreator _ray;
    [SerializeField] private NoPlacesMessage _noPlacesMessage;
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
        if (_placesHolder.TryGetPlace(cube, out ShootingPlace shootingPlace, out Vector3 escapePlace))
        {
            cube.Interect(shootingPlace, escapePlace);
            _availabilityManagement.UpdateAvailability();
        }
        else
        {
            _noPlacesMessage.DisplayMessage();
            Debug.Log("Нет доступных мест для стрельбы.");
        }
    }
}
