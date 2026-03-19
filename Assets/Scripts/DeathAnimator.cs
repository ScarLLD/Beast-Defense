using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeathAnimator : MonoBehaviour
{
    [SerializeField] DOTWeenAnimator _animator;
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private ParticleSystem _cloudParticle;
    [SerializeField] private float _deathDuration;
    [SerializeField] private float _deathDelay;

    private Coroutine _coroutine;
    private MainModule _particleModule;

    private void Awake()
    {
        _particleModule = _cloudParticle.main;
    }

    public void KillRoutine(Transform gameObject)
    {
        StartCoroutine(DeathRoutine(gameObject));
    }

    public IEnumerator DeathRoutine(Transform transform)
    {
        _animator.DoScaleDown(transform.gameObject);

        float particleTime = _cloudParticle.main.duration;
        _cloudParticle.transform.position = transform.position;
        _cloudParticle.Play();

        yield return new WaitForSeconds(particleTime + _deathDelay);
        Destroy(transform.gameObject);
    }

    public void SetParticleColor(Color color)
    {
        _particleModule.startColor = color;
    }

    private void ClearRoutine()
    {
        StopAllCoroutines();
    }
}
