using UnityEngine;

[RequireComponent(typeof(ShootingPlacesHolder))]
public class ShootingPlacesGenerator : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private ShootingPlace _placePrefab;
    [SerializeField] private int _placesCount;
    [SerializeField] private float _leftBound;
    [SerializeField] private float _rightBound;

    private ShootingPlacesHolder _holder;
    private Camera _camera;

    private void Awake()
    {
        _holder = GetComponent<ShootingPlacesHolder>();
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        _game.Started += SelectSpawnPoint;
    }

    private void OnDisable()
    {
        _game.Started -= SelectSpawnPoint;
    }

    private void SelectSpawnPoint()
    {
        Vector3 cameraCenter = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 10);

        Ray ray = _camera.ScreenPointToRay(cameraCenter);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            GeneratePlaces();
        }
    }

    private void GeneratePlaces()
    {
        float placeWeight = _placePrefab.transform.localScale.x;
        float availableSpace = _rightBound - _leftBound - (_placesCount * placeWeight);

        float spacing = availableSpace / (_placesCount - 1);

        for (int i = 0; i < _placesCount; i++)
        {
            float x = _leftBound + (placeWeight / 2) + i * (placeWeight + spacing);
            Vector3 spawnPosition = new(x, 0, 0);

            ShootingPlace place = Instantiate(_placePrefab, transform);
            place.transform.localPosition = spawnPosition;

            _holder.PutPlace(place);
        }
    }
}
