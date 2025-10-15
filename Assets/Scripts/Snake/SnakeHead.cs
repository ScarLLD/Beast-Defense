using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] private ParticleSystem _dragonFiraParticle;
    [SerializeField] private float _minParticleSpeed = 4f;
    [SerializeField] private float _maxParticleSpeed = 10f;

    private ParticleSystem.MainModule _main;
    private float _originalSpeed;

    public bool IsPlaying => _dragonFiraParticle.isPlaying;

    private void Awake()
    {
        _main = _dragonFiraParticle.main;
        _originalSpeed = _main.startSpeed.constant;
    }

    private void OnEnable()
    {
        _dragonFiraParticle.Play();
    }

    private void OnDisable()
    {
        _dragonFiraParticle.Stop();
    }

    public void SetDefaultSetting()
    {
        _main.startSpeed = _originalSpeed;
    }

    public void ChangeParticleSpeed(float snakeSpeed)
    {
        _main.startSpeed = Mathf.Clamp(_originalSpeed * snakeSpeed, _minParticleSpeed, _maxParticleSpeed);
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }
}
