using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 30f;
    [SerializeField] private float _arrivalThreshold = 0.7f;

    private ParticleCreator _particleCreator;
    private Rigidbody _rigidbody;
    private Coroutine _moveCoroutine;

    public Cube Target;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(ParticleCreator creator)
    {
        _particleCreator = creator;
    }

    public void InitTarget(Cube cube)
    {
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);

        _moveCoroutine = StartCoroutine(MoveToTarget(cube));
    }

    private IEnumerator MoveToTarget(Cube cube)
    {
        Target = cube;

        while (true)
        {
            Vector3 direction = (cube.transform.position - transform.position).normalized;
            _rigidbody.velocity = direction * _speed;

            if ((cube.transform.position - transform.position).magnitude < _arrivalThreshold)
            {
                _rigidbody.velocity = Vector3.zero;
                _particleCreator?.Create(cube);
                cube.Hit();
                break;
            }

            yield return null;
        }

        _moveCoroutine = null;
        gameObject.SetActive(false);
    }
}
