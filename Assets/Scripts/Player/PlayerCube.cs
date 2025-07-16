using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
public class PlayerCube : MonoBehaviour, ICube
{
    [SerializeField] private float _speed;

    [SerializeField] private float _outlineActive = 4.4f;
    [SerializeField] private float _outlineDisable = 0f;

    public bool IsStatic { get; private set; } = true;
    public bool IsAvailable { get; private set; } = false;
    public CubeMover Mover { get; private set; }
    public Material Material => _meshRenderer.material;
    public int Count => _shooter.BulletCount;

    private Outline _outline;
    private MeshRenderer _meshRenderer;
    private TargetRadar _radar;
    private Shooter _shooter;
    private View _view;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _meshRenderer = GetComponent<MeshRenderer>();
        Mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
        _view = GetComponent<View>();
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
        _shooter.BulletsDecreased += OnBulletsDecreased;
    }

    private void OnDisable()
    {
        Mover.Arrived -= OnMoverArrived;
        _shooter.BulletsDecreased -= OnBulletsDecreased;
    }

    public void ChangeStaticStatus(bool isStatic)
    {
        IsStatic = isStatic;
    }

    public void ChangeAvailableStatus(bool isAvailable)
    {
        IsAvailable = isAvailable;

        if (IsAvailable)
        {
            _outline.OutlineWidth = _outlineActive;
            _view.DisplayBullets();
        }
        else
            _outline.OutlineWidth = _outlineDisable;
    }

    private void OnMoverArrived()
    {
        _radar.StartScanning(Material.color);
        Mover.StopMoving();
    }

    private void OnBulletsDecreased()
    {
        if (_shooter.BulletCount == 0)
            Mover.GoEscape();
    }
}
