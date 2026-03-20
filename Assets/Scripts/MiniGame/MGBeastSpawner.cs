using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBeastSpawner : MonoBehaviour
{
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private BeastCollector _collector;
    [SerializeField] private DOTWeenAnimator _miniGameAnimator;
    [SerializeField] private Transform _spawnPlatform;
    [SerializeField] private MGBeast _beastPrefab;

    [Header("SpawnRoutine Settings")]
    [SerializeField] private Transform _container;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _boundOffset;
    [SerializeField] private float _checkRadius;
    [SerializeField] private int _minRandomBeastCount = 3;
    [SerializeField] private int _maxRandomBeastCount = 10;

    private List<MGBeast> _beasts;
    private int _maxBeastCount;
    private int _spawnAttempsCount = 50;
    private Bounds _bounds;
    private Coroutine _coroutine;
    private WaitForSeconds _sleepTime;
    private ObjectPool<MGBeast> _pool;

    private void OnEnable()
    {
        _miniGame.Started += StartRoutine;
        _miniGame.Defeat += ResetSettings;
    }

    private void OnDisable()
    {
        _miniGame.Started -= StartRoutine;
        _miniGame.Defeat -= ResetSettings;
    }

    private void Awake()
    {
        _pool = new(_beastPrefab, transform);
        _beasts = new();
        _bounds = new Bounds(_spawnPlatform.position, _spawnPlatform.localScale);
        _sleepTime = new WaitForSeconds(_spawnDelay);

        RandomizeMaxBeastCount();
    }

    public void InitializeSkin(GameObject beastPrefab)
    {
        _beastPrefab = beastPrefab.GetComponent<MGBeast>();
    }

    private Vector3 GetRandomPointInCube()
    {
        Vector3 randomPoint = new(
            Random.Range(_bounds.min.x + _boundOffset, _bounds.max.x - _boundOffset),
            _bounds.max.y,
            Random.Range(_bounds.min.z + _boundOffset, _bounds.max.z - _boundOffset)
        );

        return randomPoint;
    }

    private void StartRoutine()
    {
        ResetSettings();
        _coroutine ??= StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        int spawnedCount = 0;

        while (spawnedCount < _maxBeastCount && _miniGame.IsActive)
        {
            yield return _sleepTime;

            if (TrySpawn())
            {
                spawnedCount++;
                Debug.Log($"Заспавнен зверь №{spawnedCount} из {_maxBeastCount}");
            }
            else
            {
                Debug.Log($"Не удалось заспавнить зверя после {_spawnAttempsCount} попыток.");
            }
        }

        StopRoutine();
        Debug.Log($"Завершён спавн. Всего заспавнено: {spawnedCount}");
    }

    private bool TrySpawn()
    {
        int attempts = 0;

        while (attempts < _spawnAttempsCount && _miniGame.IsActive)
        {
            Vector3 spawnPoint = GetRandomPointInCube();

            if (CheckCollidersNearPoint(spawnPoint))
            {
                Spawn(spawnPoint);
                Debug.Log($"Заспавнил зверя. Успешная попытка №{attempts + 1}.");
                return true;
            }

            attempts++;
        }

        return false;
    }

    private bool CheckCollidersNearPoint(Vector3 spawnPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, _checkRadius);

        foreach (Collider collider in hitColliders)
        {
            if (collider.GetComponent<Beast>() != null)
            {
                return false;
            }

            if (collider.GetComponent<MGCube>() != null)
            {
                return false;
            }
        }

        return true;
    }


    private void Spawn(Vector3 spawnPoint)
    {
        var beast = _pool.GetObject();
        spawnPoint.y += _beastPrefab.transform.localScale.y;
        beast.transform.position = spawnPoint;
        beast.transform.rotation = Quaternion.LookRotation(Vector3.back);
        _miniGameAnimator.DoScaleUp(beast.gameObject);

        if (_beasts.Contains(beast) == false)
            _beasts.Add(beast);
    }

    private void StopRoutine()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    private void ResetSettings()
    {
        RandomizeMaxBeastCount();
        StopRoutine();

        foreach (var beast in _beasts)
        {
            beast.gameObject.SetActive(false);
        }

        _beasts.Clear();
    }

    private void RandomizeMaxBeastCount()
    {
        _maxBeastCount = Random.Range(_minRandomBeastCount, _maxRandomBeastCount);
        _collector.SetNewMaxBeastCount(_maxBeastCount);
    }
}
