using Game.Scripts.Pool;
using Game.Scripts.CubeCore;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class ParticleCreator : MonoBehaviour
    {
        [SerializeField] private ExplosionParticle _particlePrefab;

        private ObjectPool<ExplosionParticle> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<ExplosionParticle>(_particlePrefab, transform);
        }

        public void Create(Cube cube)
        {
            var particle = _pool.GetObject();
            particle.ChangeMaterial(cube.Material);
            particle.transform.SetPositionAndRotation(cube.transform.position, cube.transform.rotation);
        }
    }
}