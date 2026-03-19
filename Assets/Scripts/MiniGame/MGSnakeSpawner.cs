using UnityEngine;

public class MGSnakeSpawner : MonoBehaviour
{
    [SerializeField] private MiniGame _miniGame;

    [Header("SpawnRoutine settings")]
    [SerializeField] private GameObject _snakePrefab;
    [SerializeField] private MGSnake _snake;
    [SerializeField] private Vector3 _spawnPoint;

    private Vector3 _modelSpawnPoint = new Vector3(0f, -1.25f, 0f);
    private GameObject _snakeModel;

    private void OnEnable()
    {
        _miniGame.Started += SpawnRoutine;
    }

    private void OnDisable()
    {
        _miniGame.Started -= SpawnRoutine;
    }

    public void InitializeSkin(GameObject snakePrefab, Color color)
    {
        _snakePrefab = snakePrefab;
        _snake.SetBodyColor(color);
    }

    public void SpawnRoutine()
    {
        ResetSettings();
        Spawn();
        _snake.StartMove();
    }

    private void Spawn()
    {
        if (_snakeModel != null)
        {
            Destroy(_snakeModel);
            _snakeModel = null;
        }

        _snakeModel = Instantiate(_snakePrefab, _snake.transform);
        _snakeModel.transform.localPosition = _modelSpawnPoint;
    }

    private void ResetSettings()
    {
        _snake.transform.position = _spawnPoint;
        _snake.transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }
}
