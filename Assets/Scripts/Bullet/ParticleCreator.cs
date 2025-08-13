using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        particle.transform.position = cube.transform.position;
        particle.transform.rotation = cube.transform.rotation;
    }
}
