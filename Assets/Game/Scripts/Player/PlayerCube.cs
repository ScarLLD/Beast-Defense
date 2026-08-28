using System.Collections;
using System.Collections.Generic;
using Game.Scripts.BulletCore;
using Game.Scripts.CubeCore;
using Game.Scripts.MapGenerator;
using Game.Scripts.MapGenerator.Grid;
using Game.Scripts.Road;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(CubeMover))]
    [RequireComponent(typeof(TargetRadar))]
    [RequireComponent(typeof(Shooter))]
    [RequireComponent(typeof(CubeStack))]
    [RequireComponent(typeof(Animator))]
    public class PlayerCube : MonoBehaviour
    {
        private const int BULLETS_PER_SEGMENT = 4;

        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _scaleChangerSpeed = 3f;
        [SerializeField] private float _outlineActive = 4.4f;
        [SerializeField] private float _outlineDisable;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Outline _outline;
        [SerializeField] private List<MeshRenderer> _legs;
        
        private Vector3 _defaultPosition;
        private Vector3 _defaultScale;
        private BulletView _bulletView;
        private PlayerCubeAnimator _cubeAnimator;
        private GridCell _gridCell;
        private CubeMover _mover;
        private TargetRadar _radar;
        private Shooter _shooter;
        private bool _isScaled;

        public CubeStack GetStack { get; private set; }
        public bool IsAvailable { get; private set; }
        public bool HasClicked { get; private set; }
        public bool IsScaling { get; private set; }

        private void Awake()
        {
            _cubeAnimator = GetComponent<PlayerCubeAnimator>();
            _mover = GetComponent<CubeMover>();
            _shooter = GetComponent<Shooter>();
            _radar = GetComponent<TargetRadar>();
            _bulletView = GetComponent<BulletView>();
            GetStack = GetComponent<CubeStack>();
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

        public void Init(GridCell cell, Material material, int count, BulletSpawner bulletSpawner,
            TargetStorage targetStorage)
        {
            _gridCell = cell;

            _meshRenderer.material = material;
            _shooter.Init(bulletSpawner, count, BULLETS_PER_SEGMENT);
            _radar.Init(targetStorage, BULLETS_PER_SEGMENT);
            _mover.Init(_moveSpeed);
            GetStack.Init(material, count);

            foreach (var leg in _legs)
                leg.material = material;

            InitialDefaultTransform();
        }

        public void Interact(ShootingPlace shootingPlace, Vector3 escapePlace)
        {
            HasClicked = true;
            IsAvailable = false;

            _outline.OutlineWidth = _outlineDisable;
            _gridCell.ChangeStaticStatus(false);
            _bulletView.DisplayBullets();
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
            _shooter.ResetSettings();
            _mover.SetDefaultSetting();
            _bulletView.SetEmpty();

            TurnOffLegs();

            SetHalfSizeTransform();
        }

        public void ChangeAvailableStatus(bool isAvailable)
        {
            IsAvailable = isAvailable;

            if (IsAvailable)
                ActivateAvailability();
            else
                DeactivateAvailability();
        }

        private void StartMoving()
        {
            TurnOnLegs();
            _cubeAnimator.SetWalkBool(true);
            _mover.StartMoving();
        }

        private void InitialDefaultTransform()
        {
            _defaultScale = transform.localScale;
            _defaultPosition = transform.position;
        }

        private void ActivateAvailability()
        {
            if (_isScaled)
                return;

            _outline.OutlineWidth = _outlineActive;
            _bulletView.DisplayBullets();
            StartCoroutine(ScaleRoutine());
        }

        private void DeactivateAvailability()
        {
            _outline.OutlineWidth = _outlineDisable;

            if (_gridCell.IsStatic)
                _bulletView.SetEmpty();
        }

        private void SetDefaultTransform()
        {
            transform.localScale = _defaultScale;
            transform.position = _defaultPosition;

            _meshRenderer.transform.localPosition = Vector3.zero;
        }

        private void SetHalfSizeTransform()
        {
            transform.localScale = new Vector3(_defaultScale.x, _defaultScale.y / 2, _defaultScale.z);
            transform.position = new Vector3(_defaultPosition.x, _defaultPosition.y - _defaultScale.y / 4,
                _defaultPosition.z);

            _meshRenderer.transform.localPosition = Vector3.zero;
        }

        private IEnumerator ScaleRoutine()
        {
            IsScaling = true;

            var startScale = transform.localScale;
            var startPosition = transform.position;

            var progress = 0f;

            while (progress < 1f)
            {
                progress += Time.deltaTime * _scaleChangerSpeed;
                transform.localScale = Vector3.Lerp(startScale, _defaultScale, progress);
                transform.position = Vector3.Lerp(startPosition, _defaultPosition, progress);
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
            if (_shooter.BulletCount != 0) return;

            TurnOnLegs();
            _cubeAnimator.SetWalkBool(true);
            _mover.GoEscape();
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
}