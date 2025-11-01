using UnityEngine;

public class BeastSpawner : MonoBehaviour
{
    [SerializeField] private Beast _beastPrefab;

    private Beast _beast;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public Beast Spawn()
    {
        if (_beast == null)
            _beast = Instantiate(_beastPrefab, _transform);

        return _beast;
    }
}
