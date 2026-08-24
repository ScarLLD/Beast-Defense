using Game.Scripts.MapGenerator;
using Game.Scripts.Options;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class CubesInteractor : MonoBehaviour
    {
        [SerializeField] private RayCreator _ray;
        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private NoPlacesMessageDisplayer _noPlacesMessage;
        [SerializeField] private PlaceStorage _placesHolder;
        [SerializeField] private AvailabilityManagement _availabilityManagement;

        private void OnEnable()
        {
            _ray.Clicked += OnRayClicked;
        }

        private void OnDisable()
        {
            _ray.Clicked -= OnRayClicked;
        }

        private void OnRayClicked(PlayerCube cube)
        {
            if (_placesHolder.TryGetPlace(cube, out ShootingPlace shootingPlace, out Vector3 escapePlace))
            {
                _audioPlayer.PlayPickShooterSound();
                cube.Interact(shootingPlace, escapePlace);
                _availabilityManagement.UpdateAvailability();
            }
            else
            {
                _noPlacesMessage.DisplayMessage();
            }
        }
    }
}