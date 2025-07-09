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
        Mover.Arrived += OnMoverArrived;
        _shooter.BulletsOut += OnBulletsOut;
    }

    private void OnDisable()
    {
        Mover.Arrived -= OnMoverArrived;
        _shooter.BulletsOut -= OnBulletsOut;
    }

    public void ChangeStaticStatus(bool isStatic)
    {
        IsStatic = isStatic;
    }

    public void ChangeAvailableStatus(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    private void OnMoverArrived()
    {
        _radar.StartScanning(Material.color);
        Mover.StopMoving();
    }

    private void OnBulletsOut()
    {
        Mover.GoEscape();
    }
}
