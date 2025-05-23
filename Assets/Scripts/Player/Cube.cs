using UnityEngine;

[RequireComponent(typeof(CubeMover))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _maxDistance;

    public bool IsStatic { get; private set; } = true;
    public bool IsAvailable { get; private set; } = true;
    public CubeMover Mover { get; private set; }

    private TargetRadar _radar;

    private void Awake()
    {
        Mover = GetComponent<CubeMover>();
        Mover.Init(_speed, _maxDistance);
    }

    private void OnEnable()
    {
        Mover.Arrived += ActivateRadar;
    }

    private void OnDisable()
    {
        Mover.Arrived -= ActivateRadar;
    }

    public void ChangeStaticStatus()
    {
        IsStatic = !IsStatic;
    }

    private void ActivateRadar()
    {
        _radar.StartScan();
    }
}
