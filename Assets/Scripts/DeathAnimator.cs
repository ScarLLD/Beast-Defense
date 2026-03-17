using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeathAnimator : MonoBehaviour
{

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

    public IEnumerator DeathRoutine(Transform gameObject)
    {
        Vector3 startScale = gameObject.localScale;
        float timer = 0f;

        while (timer < _deathDuration)
        {
            timer += Time.deltaTime;
            float t = timer / _deathDuration;
            gameObject.localScale = Vector3.Lerp(startScale, Vector3.zero, _deathAnimationCurve.Evaluate(t));
            yield return null;
        }

        float particleTime = _cloudParticle.main.duration;
        _cloudParticle.transform.position = gameObject.position;
        _cloudParticle.Play();

        yield return new WaitForSeconds(particleTime + _deathDelay);
        Destroy(gameObject.gameObject);
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
