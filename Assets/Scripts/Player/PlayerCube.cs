using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(CubeStack))]
[RequireComponent(typeof(Animator))]
public class PlayerCube : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _transformSpeed = 10f;
    [SerializeField] private float _outlineActive = 4.4f;
    [SerializeField] private float _outlineDisable = 0f;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Outline _outline;

    private CubeStack _stack;
    private TargetRadar _radar;
    private Shooter _shooter;
    private View _view;
    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private Animator _animator;
    private bool _isScaled = false;

    public bool IsAvailable { get; private set; } = false;
    public GridCell GridCell { get; private set; }
    public CubeMover Mover { get; private set; }
    public CubeStack GetStack => _stack;
    public Material Material => _meshRenderer.material;

    private void Awake()
    {
        Mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
        _view = GetComponent<View>();
        _stack = GetComponent<CubeStack>();
        _animator = GetComponent<Animator>();
    }

    public void Init(GridCell cell, Material material, int count, BulletSpawner bulletSpawner, TargetStorage targetStorage)
    {
        GridCell = cell;

        _meshRenderer.material = material;
        _shooter.Init(bulletSpawner, count);
        _radar.Init(targetStorage);
        Mover.Init(_moveSpeed);
        _stack.Init(material, count);

        _originalScale = transform.localScale;
        _originalPosition = transform.position;

        DeactivateAvailability();
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

    public void ChangeAvailableStatus(bool isAvailable)
    {
        IsAvailable = isAvailable;

        if (IsAvailable)
            ActivateAvailability();
        else
            DeactivateAvailability();
    }

    private void ActivateAvailability()
    {
        _outline.OutlineWidth = _outlineActive;
        StartCoroutine(ScaleRoutine());
        _view.DisplayBullets();
        _animator.SetBool("isAvailable", true);
    }

    private void DeactivateAvailability()
    {
        if (_isScaled == false)
        {
            transform.localScale = new(_originalScale.x, _originalScale.y / 2, _originalScale.z);
            transform.position = new(_originalPosition.x, _originalPosition.y - transform.localScale.y / 2, _originalPosition.z);
        }

        _outline.OutlineWidth = _outlineDisable;
        _animator.SetBool("isAvailable", false);
    }

    private IEnumerator ScaleRoutine()
    {
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * _transformSpeed;
            transform.localScale = Vector3.Lerp(startScale, _originalScale, progress);
            transform.position = Vector3.Lerp(startPosition, _originalPosition, progress);
            yield return null;
        }

        transform.localScale = _originalScale;
        transform.position = _originalPosition;

        _isScaled = true;
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