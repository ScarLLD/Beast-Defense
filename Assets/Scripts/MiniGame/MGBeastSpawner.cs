using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBeastSpawner : MonoBehaviour
{
    [SerializeField] private Transform _spawnPlatform;
    [SerializeField] private GameObject _beastPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float _maxBeastCount;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _boundOffset;
    [SerializeField] private float _checkRadius;

    private List<GameObject> _beast = new();
    private int _spawnAttempsCount = 50;
    private Coroutine _coroutine;
    private WaitForSeconds _sleepTime;
    private Bounds _bounds;

    private void Awake()
    {
        _bounds = new Bounds(_spawnPlatform.position, _spawnPlatform.localScale);
        _sleepTime = new WaitForSeconds(_spawnDelay);
    }

    private void Start()
    {
        StartRoutine();
    }

    Vector3 GetRandomPointInCube()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(_bounds.min.x + _boundOffset, _bounds.max.x - _boundOffset),
            _bounds.max.y,
            Random.Range(_bounds.min.z + _boundOffset, _bounds.max.z - _boundOffset)
        );

        return randomPoint;
    }

    public void StartRoutine()
    {
        _coroutine ??= StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < _maxBeastCount; i++)
        {
            if (TrySpawn())
            {
                yield return _sleepTime;
            }
        }

        StopRoutine();
    }

    private bool TrySpawn()
    {
        int attempts = 0;

        while (attempts < _spawnAttempsCount)
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

        if (hitColliders.Length > 1)
            return false;
        else
            return true;
    }

    private void Spawn(Vector3 spawnPoint)
    {
        spawnPoint.y += _beastPrefab.transform.localScale.y;
        GameObject beast = Instantiate(_beastPrefab, spawnPoint, Quaternion.LookRotation(Vector3.back));
        _beast.Add(beast);
    }

    private void StopRoutine()
    {
        StopCoroutine(_coroutine);
        _coroutine = null;
    }
}
