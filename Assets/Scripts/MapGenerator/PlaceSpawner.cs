using UnityEngine;

[RequireComponent(typeof(PlaceStorage))]
public class PlaceSpawner : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private ShootingPlace _placePrefab;
    [SerializeField] private int _placesCount;
    [SerializeField] private float _leftBound;
    [SerializeField] private float _rightBound;
    [SerializeField] private float _movingAwayFromShootingPlace;

    private PlaceStorage _storage;
    private Camera _camera;

    private void Awake()
    {
        _storage = GetComponent<PlaceStorage>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _game.Started += OnGameStarted;
    }

    private void OnDisable()
    {
        _game.Started -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        Vector3 cameraCenter = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 10);

        Ray ray = _camera.ScreenPointToRay(cameraCenter);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            GenerateShootingPlaces(hit.point);
            GenerateEscapePlaces();
        }
    }

    private void GenerateShootingPlaces(Vector3 hit)
    {
        float placeWeight = _placePrefab.transform.localScale.x;
        float availableSpace = _rightBound - _leftBound - (_placesCount * placeWeight);

        float spacing = availableSpace / (_placesCount - 1);

        for (int i = 0; i < _placesCount; i++)
        {
            float x = _leftBound + (placeWeight / 2) + i * (placeWeight + spacing);
            Vector3 spawnPosition = new(x, hit.y + 0.01f, 0);

            ShootingPlace place = Instantiate(_placePrefab, transform);
            place.transform.localPosition = spawnPosition;

            _storage.PutPlace(place);
        }
    }

    private void GenerateEscapePlaces()
    {
        Vector3 escapePlace;

        if (_storage.TryGetFirstPlace(out ShootingPlace firstPlace))
        {
            escapePlace = new Vector3(firstPlace.transform.position.x - _movingAwayFromShootingPlace,
                firstPlace.transform.position.y, firstPlace.transform.position.z + _movingAwayFromShootingPlace);

            _storage.PutEscapePlace(escapePlace);
        }

        if (_storage.TryGetLastPlace(out ShootingPlace lastPlace))
        {
            escapePlace = new Vector3(lastPlace.transform.position.x + _movingAwayFromShootingPlace,
                lastPlace.transform.position.y, lastPlace.transform.position.z + _movingAwayFromShootingPlace);

            _storage.PutEscapePlace(escapePlace);
        }
    }
}
