using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Particle : MonoBehaviour
{
    private ParticleSystem _particle;

    private void Awake()
    {
        _particle = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        _particle.Play();
    }

    private void OnDisable()
    {
        _particle.Stop();
    }
}
