using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 45f;
    [SerializeField] private float _arrivalThreshold = 0.7f;
    [SerializeField] private BulletTrail _bulletTrail;

    private ParticleCreator _particleCreator;
    private Transform _transform;
    private Rigidbody _rigidbody;
    private Coroutine _moveCoroutine;
    private bool _isMove;

    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void StopMove()
    {
        _isMove = false;
    }

    public void Init(ParticleCreator creator)
    {
        _particleCreator = creator;
    }

    public void InitTarget(Cube cube)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _bulletTrail.ResetTrail();
        _moveCoroutine = StartCoroutine(MoveToTarget(cube));
    }

    private IEnumerator MoveToTarget(Cube cube)
    {
        _isMove = true;

        while (_isMove && cube != null && cube.isActiveAndEnabled == true)
        {
            Vector3 direction = (cube.transform.position - _transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            if ((cube.transform.position - _transform.position).magnitude < _arrivalThreshold)
            {
                if (_particleCreator != null)
                    _particleCreator.Create(cube);

                _isMove = false;
                cube.Hit();
            }

            yield return null;
        }

        _rigidbody.velocity = Vector3.zero;
        _moveCoroutine = null;
        gameObject.SetActive(false);
    }
}
