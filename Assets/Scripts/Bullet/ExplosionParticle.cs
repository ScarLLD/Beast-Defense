using UnityEngine;

public class ExplosionParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private ParticleSystemRenderer _renderer;

    private void Awake()
    {
        _renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
    }

    private void OnEnable()
    {
        _particleSystem.Play();
    }

    private void OnDisable()
    {
        _particleSystem.Stop();
    }

    public void ChangeMaterial(Material material)
    {
        _renderer.material = material;
    }
}