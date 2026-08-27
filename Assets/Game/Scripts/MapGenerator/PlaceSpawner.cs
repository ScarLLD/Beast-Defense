using UnityEngine;

namespace Game.Scripts.MapGenerator
{
    public class PlaceSpawner : MonoBehaviour
    {
        [SerializeField] private Game _game;
        [SerializeField] private PlaceStorage _storage;
        [SerializeField] private ShootingPlace _placePrefab;
        [SerializeField] private float _distanceBetweenPlaces = 1;
        [SerializeField] private int _placesCount = 4;

        private int _initialPlacesCount;

        public bool PlacesIncreased { get; private set; }

        private void Awake()
        {
            _initialPlacesCount = _placesCount;
            PlacesIncreased = false;
        }

        private void OnEnable()
        {
            _game.Completed += SetDefaultSettings;
        }

        private void OnDisable()
        {
            _game.Completed -= SetDefaultSettings;
        }

        public void GeneratePlaces()
        {
            _storage.Clear();
            GenerateShootingPlaces();
        }

        public void IncreasePlace()
        {
            _placesCount++;
            PlacesIncreased = true;
        }

        public void SetDefaultSettings()
        {
            _placesCount = _initialPlacesCount;
            PlacesIncreased = false;
        }

        private void GenerateShootingPlaces()
        {
            var placeWidth = _placePrefab.transform.localScale.x;
            var totalWidth = (_placesCount - 1) * (placeWidth + _distanceBetweenPlaces);

            var startPoint = Vector3.zero;
            startPoint.x -= totalWidth / 2;

            for (var i = 0; i < _placesCount; i++)
            {
                var spawnPosition = startPoint;
                spawnPosition.x = startPoint.x + i * (placeWidth + _distanceBetweenPlaces);

                var place = Instantiate(_placePrefab, transform);
                place.transform.localPosition = spawnPosition;
                _storage.PutPlace(place);
            }
        }
    }
}