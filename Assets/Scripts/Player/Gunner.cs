using UnityEngine;

[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(TargetRadar))]
public class Gunner : MonoBehaviour
{
    private Shooter _shooter;
    private TargetRadar _targetRadar;

    private void Awake()
    {
        _shooter = GetComponent<Shooter>();
        _targetRadar = GetComponent<TargetRadar>();
    }

    private void OnEnable()
    {
        _targetRadar.OnTargetFound += SelectTarget;
    }

    private void OnDisable()
    {
        _targetRadar.OnTargetFound -= SelectTarget;
    }

    private void SelectTarget(Transform target)
    {
        _shooter.Shoot(target);
    }
}
