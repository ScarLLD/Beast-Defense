using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(CubeStack))]
public class PlayerCube : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _outlineActive = 4.4f;
    [SerializeField] private float _outlineDisable = 0f;

    private Outline _outline;
    private CubeStack _stack;
    private MeshRenderer _meshRenderer;
    private TargetRadar _radar;
    private Shooter _shooter;
    private View _view;

    public bool IsStatic { get; private set; } = true;
    public bool IsAvailable { get; private set; } = false;
    public GridCell GridCell { get; private set; }
    public CubeMover Mover { get; private set; }
    public CubeStack GetStack => _stack;
    public Material Material => _meshRenderer.material;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _meshRenderer = GetComponent<MeshRenderer>();
        Mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
        _view = GetComponent<View>();
        _stack = GetComponent<CubeStack>();
    }

    public void Init(GridCell cell, Material material, int count, BulletSpawner bulletSpawner, TargetStorage targetStorage)
    {
        GridCell = cell;
        _meshRenderer.material = material;
        _shooter.Init(bulletSpawner, count);
        _radar.Init(targetStorage);
        Mover.Init(_speed);
        _stack.Init(material, count);
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
        {
            _outline.OutlineWidth = _outlineDisable;
        }
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
