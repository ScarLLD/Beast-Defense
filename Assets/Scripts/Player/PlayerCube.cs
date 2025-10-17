using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CubeMover))]
[RequireComponent(typeof(TargetRadar))]
[RequireComponent(typeof(Shooter))]
[RequireComponent(typeof(CubeStack))]
[RequireComponent(typeof(Animator))]
public class PlayerCube : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _scaleChangerSpeed = 3f;
    [SerializeField] private float _outlineActive = 4.4f;
    [SerializeField] private float _outlineDisable = 0f;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Outline _outline;
    [SerializeField] private List<MeshRenderer> _legs;

    private Vector3 _defaultScale;
    private Vector3 _defaultPosition;

    private PlayerCubeAnimator _cubeAnimator;
    private Transform _transform;
    private CubeStack _stack;
    private TargetRadar _radar;
    private Shooter _shooter;
    private View _view;
    private GridCell _gridCell;
    private CubeMover _mover;
    private bool _isScaled;

    public bool IsAvailable { get; private set; }
    public bool HasClicked { get; private set; }
    public bool IsScaling { get; private set; }
    public CubeStack GetStack => _stack;


    private void Awake()
    {
        _transform = transform;

        _cubeAnimator = GetComponent<PlayerCubeAnimator>();
        _mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
        _view = GetComponent<View>();
        _stack = GetComponent<CubeStack>();
    }

    public void Init(GridCell cell, Material material, int count, BulletSpawner bulletSpawner, TargetStorage targetStorage)
    {
        _gridCell = cell;

        _meshRenderer.material = material;
        _shooter.Init(bulletSpawner, count);
        _radar.Init(targetStorage);
        _mover.Init(_moveSpeed);
        _stack.Init(material, count);

        foreach (var leg in _legs)
            leg.material = material;

        InitialDefaultTransform();
    }

    private void OnEnable()
    {
        _mover.Arrived += OnMoverArrived;
        _mover.Escaped += OnMoverEscaped;
        _shooter.BulletsCountChanged += OnBulletsDecreased;
    }

    private void OnDisable()
    {
        _mover.Arrived -= OnMoverArrived;
        _mover.Escaped -= OnMoverEscaped;
        _shooter.BulletsCountChanged -= OnBulletsDecreased;
    }

    public void Interect(ShootingPlace shootingPlace, Vector3 escapePlace)
    {
        HasClicked = true;
        IsAvailable = false;

        _outline.OutlineWidth = _outlineDisable;
        _gridCell.ChangeStaticStatus(false);
        _view.DisplayBullets();
        _mover.SetPlaces(shootingPlace, escapePlace, _gridCell);
        StartMoving();
    }

    public void SetDefaultSettings()
    {
        IsAvailable = false;
        HasClicked = false;
        IsScaling = false;
        _isScaled = false;

        _cubeAnimator.ResetSettings();
        _cubeAnimator.EnableAnimator(false);

        _radar.TurnOff();
        _shooter.SetDafaultSettings();
        _mover.SetDefaultSetting();
        _view.SetEmpty();

        TurnOffLegs();

        SetHalfSizeTransform();
    }

    private void InitialDefaultTransform()
    {
        _defaultScale = _transform.localScale;
        _defaultPosition = _transform.position;
    }

    public void StartMoving()
    {
        TurnOnLegs();
        _cubeAnimator.SetWalkBool(true);
        _mover.StartMoving();
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
        if (_isScaled == false)
        {
            _outline.OutlineWidth = _outlineActive;
            _view.DisplayBullets();
            StartCoroutine(ScaleRoutine());
        }
    }

    private void DeactivateAvailability()
    {
        _outline.OutlineWidth = _outlineDisable;

        if (_gridCell.IsStatic)
            _view.SetEmpty();
    }


    private void SetDefaultTransform()
    {
        _transform.localScale = _defaultScale;
        _transform.position = _defaultPosition;

        _meshRenderer.transform.localPosition = Vector3.zero;
    }

    private void SetHalfSizeTransform()
    {
        _transform.localScale = new(_defaultScale.x, _defaultScale.y / 2, _defaultScale.z);
        _transform.position = new(_defaultPosition.x, _defaultPosition.y - _defaultScale.y / 4, _defaultPosition.z);

        _meshRenderer.transform.localPosition = Vector3.zero;
    }

    private IEnumerator ScaleRoutine()
    {
        IsScaling = true;

        Vector3 startScale = _transform.localScale;
        Vector3 startPosition = _transform.position;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * _scaleChangerSpeed;
            _transform.localScale = Vector3.Lerp(startScale, _defaultScale, progress);
            _transform.position = Vector3.Lerp(startPosition, _defaultPosition, progress);
            yield return null;
        }

        SetDefaultTransform();

        _cubeAnimator.EnableAnimator(true);
        _cubeAnimator.SetAvailableTrigger();

        _isScaled = true;
        IsScaling = false;
        yield return null;
    }

    private void OnMoverArrived()
    {
        _cubeAnimator.SetWalkBool(false);
        TurnOffLegs();
        _radar.StartScanning(_meshRenderer.material.color);
    }

    private void OnBulletsDecreased()
    {
        if (_shooter.BulletCount == 0)
        {
            TurnOnLegs();
            _cubeAnimator.SetWalkBool(true);
            _mover.GoEscape();
        }
    }

    private void OnMoverEscaped()
    {
        _cubeAnimator.ResetSettings();
        _cubeAnimator.EnableAnimator(false);
        gameObject.SetActive(false);
    }

    private void TurnOffLegs()
    {
        foreach (var leg in _legs)
            leg.gameObject.SetActive(false);
    }

    private void TurnOnLegs()
    {
        foreach (var leg in _legs)
            leg.gameObject.SetActive(true);
    }
}