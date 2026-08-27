using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.CubeCore;
using Game.Scripts.Effects;
using Game.Scripts.Options;
using Game.Scripts.Pool;
using UnityEngine;

namespace Game.Scripts.BulletCore
{
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleCreator _particleCreator;
        [SerializeField] private Bullet _bulletPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private AudioPlayer _audioPlayer;

        private List<Bullet> _bullets;

        private ObjectPool<Bullet> _pool;

        public event Action Shooting;

        private void Awake()
        {
            _pool = new ObjectPool<Bullet>(_bulletPrefab, _container);
            _bullets = new List<Bullet>();
        }

        public void SpawnBullet(Vector3 spawnPosition, Cube cube)
        {
            if (!cube)
                throw new ArgumentException("cube не может быть null.", nameof(cube));

            var bullet = _pool.GetObject();

            if (!_bullets.Contains(bullet))
                _bullets.Add(bullet);

            bullet.transform.position = spawnPosition;
            bullet.Init(_particleCreator, _audioPlayer);
            bullet.InitTarget(cube);

            Shooting?.Invoke();
        }

        public void Cleanup()
        {
            foreach (var bullet in _bullets.Where(bullet => bullet.gameObject.activeInHierarchy == true))
            {
                bullet.StopMove();
            }
        }
    }
}