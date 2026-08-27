using System.Collections;
using System.Collections.Generic;
using Game.Scripts.BeastCore;
using Game.Scripts.Pool;
using UnityEngine;

namespace Game.Scripts.MiniGameCore
{
    public class MGBeastSpawner : MonoBehaviour
    {
        private const int MAX_COLLIDERS_COUNT = 50;
        private const int SPAWN_ATTEMPS_COUNT = 50;

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
        private Bounds _bounds;
        private Coroutine _coroutine;
        private int _maxBeastCount;
        private ObjectPool<MGBeast> _pool;
        private WaitForSeconds _sleepTime;

        private Collider[] _tempColliders;

        private void Awake()
        {
            _pool = new ObjectPool<MGBeast>(_beastPrefab, transform);
            _beasts = new List<MGBeast>();
            _bounds = new Bounds(_spawnPlatform.position, _spawnPlatform.localScale);
            _sleepTime = new WaitForSeconds(_spawnDelay);
            _tempColliders = new Collider[MAX_COLLIDERS_COUNT];

            RandomizeMaxBeastCount();
        }

        private void OnEnable()
        {
            _miniGame.Started += OnMiniGameStarted;
            _miniGame.Defeated += OnMiniGameDefeated;
        }

        private void OnDisable()
        {
            _miniGame.Started -= OnMiniGameStarted;
            _miniGame.Defeated -= OnMiniGameDefeated;
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

        private void OnMiniGameStarted()
        {
            ResetSettings();
            _coroutine ??= StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            var spawnedCount = 0;

            while (spawnedCount < _maxBeastCount && _miniGame.IsActive)
            {
                yield return _sleepTime;

                if (TrySpawn()) spawnedCount++;
            }

            StopRoutine();
        }

        private bool TrySpawn()
        {
            var attempts = 0;

            while (attempts < SPAWN_ATTEMPS_COUNT && _miniGame.IsActive)
            {
                var spawnPoint = GetRandomPointInCube();

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
            var colliderCount = Physics.OverlapSphereNonAlloc(spawnPoint, _checkRadius, _tempColliders);

            for (var i = 0; i < colliderCount; i++)
            {
                var tempCollider = _tempColliders[i];

                if (tempCollider.GetComponent<Beast>() ||
                    tempCollider.GetComponent<MGCube>())
                    return false;
            }

            return true;
        }

        private void Spawn(Vector3 spawnPoint)
        {
            var beast = _pool.GetObject();
            spawnPoint.y += _beastPrefab.transform.localScale.y;
            beast.transform.SetPositionAndRotation(spawnPoint, Quaternion.LookRotation(Vector3.back));
            DOTWeenAnimator.DoScaleUp(beast.gameObject);

            if (!_beasts.Contains(beast))
                _beasts.Add(beast);
        }

        private void StopRoutine()
        {
            if (_coroutine == null) return;

            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        private void ResetSettings()
        {
            RandomizeMaxBeastCount();
            StopRoutine();

            foreach (var beast in _beasts) beast.gameObject.SetActive(false);

            _beasts.Clear();
        }

        private void OnMiniGameDefeated()
        {
            ResetSettings();
        }

        private void RandomizeMaxBeastCount()
        {
            _maxBeastCount = Random.Range(_minRandomBeastCount, _maxRandomBeastCount);
            _collector.SetNewMaxBeastCount(_maxBeastCount);
        }
    }
}