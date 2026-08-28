using System.Collections;
using Game.Scripts.MiniGameCore;
using Game.Scripts.Options;
using Game.Scripts.Pool;
using UnityEngine;

namespace Game.Scripts.Effects
{
    public class DeathAnimator : MonoBehaviour
    {
        [Header("Animator settings.")] [SerializeField]
        private DOTWeenAnimator _animator;

        [SerializeField] private AudioPlayer _audioPlayer;
        [SerializeField] private AnimationCurve _deathAnimationCurve;
        [SerializeField] private ParticleData _cloudParticlePrefab;
        [SerializeField] private float _deathDuration;
        [SerializeField] private float _deathDelay;

        private WaitForSeconds _deathTime;
        private WaitForSeconds _delayTime;

        private ObjectPool<ParticleData> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<ParticleData>(_cloudParticlePrefab, transform);

            _deathTime = new WaitForSeconds(DOTWeenAnimator.GetDuration);
            _delayTime = new WaitForSeconds(_cloudParticlePrefab.GetDuration + _deathDelay);
        }

        public void KillRoutine(Transform target, Color color)
        {
            StartCoroutine(DeathRoutine(target, color));
        }

        public IEnumerator DeathRoutine(Transform target, Color color)
        {
            DOTWeenAnimator.DoScaleDown(target.gameObject);
            yield return _deathTime;
            
            _audioPlayer.PlayCloudParticleSound();

            target.gameObject.SetActive(false);
            var cloudParticle = _pool.GetObject();
            cloudParticle.SetColor(color);
            cloudParticle.transform.position = target.position;

            yield return _delayTime;
        }
    }
}