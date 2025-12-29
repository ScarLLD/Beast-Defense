using UnityEngine;

public class PlaceSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Camera _camera;
    [SerializeField] private PlaceStorage _storage;
    [SerializeField] private ShootingPlace _placePrefab;
    [SerializeField] private float _distanceBetweenPlaces = 1;
    [SerializeField] private int _placesCount = 4;
    [SerializeField] private float _movingAwayFromShootingPlace = 10;
    [SerializeField] private float _scaleMultiplier = 0.9f;

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

    public bool TryGeneratePlaces()
    {
        _storage.Clear();
        GenerateShootingPlaces();

        return true;
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
        float placeWidth = _placePrefab.transform.localScale.x;
        float totalWidth = (_placesCount - 1) * (placeWidth + _distanceBetweenPlaces);

        Vector3 startPoint = Vector3.zero;
        startPoint.x -= totalWidth / 2;

        for (int i = 0; i < _placesCount; i++)
        {
            Vector3 spawnPosition = startPoint;
            spawnPosition.x = startPoint.x + i * (placeWidth + _distanceBetweenPlaces);

            ShootingPlace place = Instantiate(_placePrefab, transform);
            place.transform.localPosition = spawnPosition;
            _storage.PutPlace(place);
        }
    }
}
