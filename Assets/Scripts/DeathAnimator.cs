using System.Collections;
using UnityEngine;

public class DeathAnimator : MonoBehaviour
{
    [Header("Animator settings.")]
    [SerializeField] private DOTWeenAnimator _animator;
    [SerializeField] private AnimationCurve _deathAnimationCurve;
    [SerializeField] private Particle _cloudParticlePrefab;
    [SerializeField] private float _deathDuration;
    [SerializeField] private float _deathDelay;

    private WaitForSeconds _deathTime;
    private WaitForSeconds _delayTime;

    private ObjectPool<Particle> _pool;


    private void Awake()
    {
        _pool = new(_cloudParticlePrefab, transform);

        _deathTime = new WaitForSeconds(_animator.GetDuration);
        _delayTime = new WaitForSeconds(_cloudParticlePrefab.GetDuration + _deathDelay);
    }

    public void KillRoutine(Transform gameObject, Color color)
    {
        StartCoroutine(DeathRoutine(gameObject, color));
    }

    public IEnumerator DeathRoutine(Transform transform, Color color)
    {
        _animator.DoScaleDown(transform.gameObject);
        yield return _deathTime;


        var cloudParticle = _pool.GetObject();
        cloudParticle.SetColor(color);
        cloudParticle.transform.position = transform.position;

        yield return _delayTime;
        Destroy(transform.gameObject);

        ClearRoutine();
    }

    private void ClearRoutine()
    {
        StopAllCoroutines();
    }
}
