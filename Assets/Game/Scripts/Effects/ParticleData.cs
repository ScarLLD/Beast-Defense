using System.Collections;
using UnityEngine;

namespace Game.Scripts.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleData : MonoBehaviour
    {
        private ParticleSystem _particle;
        private ParticleSystem.MainModule _particleModule;
        private Coroutine _disableCoroutine;
        private WaitForSeconds _sleep;

        public float GetDuration => _particleModule.duration;

        private void Awake()
        {
            _particle = GetComponent<ParticleSystem>();
            _particleModule = _particle.main;
            _sleep = new WaitForSeconds(_particleModule.duration);
        }

        private void OnEnable()
        {
            if (_disableCoroutine != null)
                StopCoroutine(_disableCoroutine);

            _particle.Play();
            _disableCoroutine = StartCoroutine(WaitAndDisable());
        }

        private void OnDisable()
        {
            if (_disableCoroutine != null)
                StopCoroutine(_disableCoroutine);

            _particle.Stop();
        }

        private IEnumerator WaitAndDisable()
        {
            var duration = _particleModule.duration;

            yield return _sleep;

            gameObject.SetActive(false);
        }

        public void SetColor(Color color)
        {
            _particleModule.startColor = color;
        }
    }
}