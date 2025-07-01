using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
public class PlayerCube : MonoBehaviour, ICube
{
    [SerializeField] private float _speed;

    public bool IsStatic { get; private set; } = true;
    public bool IsAvailable { get; private set; } = true;
    public CubeMover Mover { get; private set; }

    public int Count => _shooter.BulletCount;

    public Material Material => _meshRenderer.material;

    private MeshRenderer _meshRenderer;
    private TargetRadar _radar;
    private Shooter _shooter;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        Mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
    }

    public void Init(Material material, int bulletCount, BulletSpawner bulletSpawner, TargetStorage targetStorage)
    {
        _meshRenderer.material = material;
        _shooter.Init(bulletSpawner, bulletCount);
        _radar.Init(targetStorage);
        Mover.Init(_speed);
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
        _radar.StartScanning(Material.color);
        Mover.StopMoving();
    }
}
