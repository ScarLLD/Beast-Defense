using UnityEngine;

public class Gunner : MonoBehaviour
{
    [SerializeField] private Shooter _shooter;
    [SerializeField] private TargetRadar _targetRadar;

    private void OnEnable()
    {
        _targetRadar.Found += SelectTarget;
    }

    private void OnDisable()
    {
        _targetRadar.Found -= SelectTarget;
    }

    private void SelectTarget(Transform target)
    {
        _shooter.Shoot(target);
    }
}
