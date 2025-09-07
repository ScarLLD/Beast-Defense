using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BulletTrail : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        ResetTrail();
    }

    private void OnDisable()
    {
        ResetTrail();
    }

    public void ResetTrail()
    {
        if (_trailRenderer != null)
        {            
            _trailRenderer.Clear();
        }
    }
}
