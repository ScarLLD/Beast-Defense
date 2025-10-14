using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

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

    private CubeStack _stack;
    private TargetRadar _radar;
    private Shooter _shooter;
    private View _view;
    private Vector3 _originalScale;
    private Vector3 _originalPosition;
    private Animator _animator;
    private GridCell _gridCell;
    private CubeMover _mover;
    private bool _isScaled = false;

    public bool IsAvailable { get; private set; } = false;
    public bool HasClicked { get; private set; } = false;
    public bool IsScaling { get; private set; } = false;
    public CubeStack GetStack => _stack;


    private void Awake()
    {
        _mover = GetComponent<CubeMover>();
        _shooter = GetComponent<Shooter>();
        _radar = GetComponent<TargetRadar>();
        _view = GetComponent<View>();
        _stack = GetComponent<CubeStack>();
        _animator = GetComponent<Animator>();
    }

    public void Init(GridCell cell, Material material, int count, BulletSpawner bulletSpawner, TargetStorage targetStorage)
    {
        _gridCell = cell;

        _meshRenderer.material = material;
        _shooter.Init(bulletSpawner, count);
        _radar.Init(targetStorage);
        _mover.Init(_moveSpeed);
        _stack.Init(material, count);

        _originalScale = transform.localScale;
        _originalPosition = transform.position;

        _animator.enabled = false;

        foreach (var leg in _legs)
            leg.material = material;

        DeactivateAvailability();
    }

    private void OnEnable()
    {
        _mover.Arrived += OnMoverArrived;
        _shooter.BulletsCountChanged += OnBulletsDecreased;
    }

    private void OnDisable()
    {
        _mover.Arrived -= OnMoverArrived;
        _shooter.BulletsCountChanged -= OnBulletsDecreased;

        _animator.SetBool("isWalk", false);
        _animator.enabled = false;
    }

    public void Interect(ShootingPlace shootingPlace, Vector3 escapePlace)
    {
        HasClicked = true;
        _mover.SetPlaces(shootingPlace, escapePlace, _gridCell);
        _gridCell.ChangeStaticStatus(false);
        ChangeAvailableStatus(false);
        TurnOnLegs();
        StartMoving();
    }

    public void SetDefaultSettings()
    {
        HasClicked = false;
        _isScaled = false;

        transform.position = _originalPosition;
        _meshRenderer.transform.position = _originalPosition;

        _animator.enabled = false;

        _radar.TurnOff();
        _shooter.SetDafaultSettings();
        _mover.SetDefaultSetting();
        TurnOffLegs();
    }

    public void StartMoving()
    {
        _animator.SetBool("isWalk", true);
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
            StartCoroutine(ScaleRoutine());
            _view.DisplayBullets();
        }
        
    }

    private void DeactivateAvailability()
    {

        _outline.OutlineWidth = _outlineDisable;

        if (_gridCell.IsStatic)
            _view.SetEmpty();

        foreach (var leg in _legs)
            leg.gameObject.SetActive(false);

        if (_isScaled == false)
        {
            SetHalfSizeTransform();
        }
    }

    private void SetHalfSizeTransform()
    {
        transform.localScale = new(_originalScale.x, _originalScale.y / 2, _originalScale.z);
        transform.position = new(_originalPosition.x, _originalPosition.y - transform.localScale.y / 2, _originalPosition.z);
    }

    private IEnumerator ScaleRoutine()
    {
        IsScaling = true;

        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * _scaleChangerSpeed;
            transform.localScale = Vector3.Lerp(startScale, _originalScale, progress);
            transform.position = Vector3.Lerp(startPosition, _originalPosition, progress);
            yield return null;
        }

        transform.localScale = _originalScale;
        transform.position = _originalPosition;

        _animator.enabled = true;
        _animator.SetTrigger("isAvailable");

        _isScaled = true;
        IsScaling = false;
        yield return null;
    }

    private void OnMoverArrived()
    {
        _animator.SetBool("isWalk", false);
        TurnOffLegs();
        _radar.StartScanning(_meshRenderer.material.color);
    }

    private void OnBulletsDecreased()
    {
        if (_shooter.BulletCount == 0)
        {
            TurnOnLegs();
            _animator.SetBool("isWalk", true);
            _mover.GoEscape();
        }
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