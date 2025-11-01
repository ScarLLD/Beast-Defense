using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeathModule : MonoBehaviour
{
    [SerializeField] private Game _game;

    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private ParticleSystem _cloudParticle;
    [SerializeField] private float _deathDuration;
    [SerializeField] private float _deathDelay;

    private MainModule _particleModule;

    public event Action BeastDie;
    public event Action SnakeDie;

    private void Awake()
    {
        _particleModule = _cloudParticle.main;
    }

    public void KillSnake(Transform gameObject)
    {
        StartCoroutine(KillSnakeRoutine(gameObject));
    }

    public void KillBeast(Transform gameObject)
    {
        StartCoroutine(KillBeastRoutine(gameObject));
    }

    private IEnumerator KillSnakeRoutine(Transform gameObject)
    {
        _particleModule.startColor = Color.red;
        yield return StartCoroutine(DeathRoutine(gameObject));
        SnakeDie?.Invoke();
    }

    private IEnumerator KillBeastRoutine(Transform gameObject)
    {
        _particleModule.startColor = Color.white;
        yield return StartCoroutine(DeathRoutine(gameObject));
        BeastDie?.Invoke();        
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
    }
}
