using System;
using UnityEngine;

[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
public class PlayerCube : MonoBehaviour
{
    [SerializeField] private float _speed;
    [Range(0, 0.1f)]
    [SerializeField] private float _maxDistanceRounding;

    public bool IsStatic { get; private set; } = true;
    public bool IsAvailable { get; private set; } = true;
    public CubeMover Mover { get; private set; }

    private TargetRadar _radar;
    private Shooter _shooter;

    private void Awake()
    {
        Mover = GetComponent<CubeMover>();
        _radar = GetComponent<TargetRadar>();
        _shooter = GetComponent<Shooter>();
        Mover.Init(_speed, _maxDistanceRounding);
    }

    public void Init(BulletSpawner bulletSpawner)
    {
        _shooter.Init(bulletSpawner);
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

    public void ChangeAvailableStatus()
    {
        IsAvailable = !IsAvailable;
    }

    private void ActivateRadar()
    {
        _radar.StartScanning();
    }
}
