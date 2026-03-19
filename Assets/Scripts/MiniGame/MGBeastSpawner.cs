using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBeastSpawner : MonoBehaviour
{
    [SerializeField] private MiniGame _miniGame;
    [SerializeField] private DOTWeenAnimator _miniGameAnimator;
    [SerializeField] private Transform _spawnPlatform;
    [SerializeField] private GameObject _beastPrefab;

    [Header("SpawnRoutine Settings")]
    [SerializeField] private Transform _container;
    [SerializeField] private float _maxBeastCount;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _boundOffset;
    [SerializeField] private float _checkRadius;

    private readonly List<GameObject> _beasts = new();
    private int _spawnAttempsCount = 50;
    private int _allowedCountColliders = 2;
    private Bounds _bounds;
    private Coroutine _coroutine;
    private WaitForSeconds _sleepTime;

    private void OnEnable()
    {
        _miniGame.Started += StartRoutine;
    }

    private void OnDisable()
    {
        _miniGame.Started -= StartRoutine;
    }

    private void Awake()
    {
        _bounds = new Bounds(_spawnPlatform.position, _spawnPlatform.localScale);
        _sleepTime = new WaitForSeconds(_spawnDelay);
    }

    public void InitializeSkin(GameObject beastPrefab)
    {
        _beastPrefab = beastPrefab;
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
        for (int i = 0; i < _maxBeastCount && _miniGame.IsActive; i++)
        {
            if (TrySpawn())
            {
                yield return _sleepTime;
            }
        }

        yield return null;
        StopRoutine();
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
                return true;
            }

            attempts++;
        }

        return false;
    }

    private bool CheckCollidersNearPoint(Vector3 spawnPoint)
    {
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, _checkRadius);

        if (hitColliders.Length > _allowedCountColliders)
            return false;
        else
            return true;
    }

    private void Spawn(Vector3 spawnPoint)
    {
        spawnPoint.y += _beastPrefab.transform.localScale.y;
        GameObject beast = Instantiate(_beastPrefab, spawnPoint, Quaternion.LookRotation(Vector3.back), _container);
        beast.transform.localScale = Vector3.zero;
        _miniGameAnimator.DoScaleUp(beast);

        beast.AddComponent<Beast>();
        beast.AddComponent<BoxCollider>().isTrigger = true;
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
        StopRoutine();

        foreach (var beast in _beasts)
        {
            Destroy(beast.gameObject);
        }

        _beasts.Clear();
    }
}
