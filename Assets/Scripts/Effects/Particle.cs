using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ParticleSystem))]
public class Particle : MonoBehaviour
{
    private ParticleSystem _particle;
    private MainModule _particleModule;

    public float GetDuration => _particleModule.duration; 

    private void OnEnable()
    {
        _particle.Play();
    }

    private void OnDisable()
    {
        _particle.Stop();
    }

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
        _particleModule = _particle.main;
    }

    public void SetColor(Color color)
    {
        _particleModule.startColor = color;
    }
}
