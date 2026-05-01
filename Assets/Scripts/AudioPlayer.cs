using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioSource _musicSource;

    [Header("Sounds")]
    [SerializeField] private AudioClip _transitionSound;
    [SerializeField] private AudioClip _beastJumpSound;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _noPlacesMessageSound;
    [SerializeField] private AudioClip _skinBuySound;
    [SerializeField] private AudioClip _skinSelectSound;
    [SerializeField] private AudioClip _buttonClickSound;

    [Header("Musics")]
    [SerializeField] private List<AudioClip> _musics;

    [Header("Other")]
    [SerializeField] private Game _game;
    [SerializeField] private SkinShop _shop;
    [SerializeField] private Transition _transition;
    [SerializeField] private BulletSpawner _bulletSpawner;
    [SerializeField] private NoPlacesMessageDisplayer _noPlacesMessageDisplayer;

    private void OnEnable()
    {
        _game.Started += OnGameStarted;
        _game.Restarted += OnGameRestarted;
        _game.Continued += OnGameContinued;
        _game.Completed += OnGameCompleted;
        _game.Loss += OnGameLoss;
        _game.Leaved += OnGameLeaved;

        _shop.Purchased += OnPurchasedSkin;
        _shop.Selected += OnSelectedSkin;

        _transition.Transiting += OnTransiting;
        _bulletSpawner.Shooting += OnShooting;
        _noPlacesMessageDisplayer.Displayed += OnMessageDisplayed;
        Window.ButtonClicked += OnButtonClicked;
    }

    private void OnDisable()
    {
        _game.Started -= OnGameStarted;
        _game.Restarted -= OnGameRestarted;
        _game.Continued -= OnGameContinued;
        _game.Completed -= OnGameCompleted;
        _game.Loss -= OnGameLoss;
        _game.Leaved -= OnGameLeaved;

        _shop.Purchased -= OnPurchasedSkin;
        _shop.Selected -= OnSelectedSkin;

        _transition.Transiting -= OnTransiting;
        _bulletSpawner.Shooting -= OnShooting;
        _noPlacesMessageDisplayer.Displayed -= OnMessageDisplayed;
        Window.ButtonClicked -= OnButtonClicked;
    }

    public void PlayHitSound()
    {
        PlaySound(_hitSound);
    }

    public void OnButtonClicked()
    {
        PlaySound(_buttonClickSound);
    }

    private void OnTransiting()
    {
        PlaySound(_transitionSound);
    }

    private void OnMessageDisplayed()
    {
        PlaySound(_noPlacesMessageSound);
    }

    private void OnShooting()
    {
        PlaySound(_shootSound);
    }

    public void PlayBeastJumpSound()
    {
        PlaySound(_beastJumpSound);
    }

    private void PlaySound(AudioClip clip)
    {
        _soundSource.PlayOneShot(clip);
    }

    private void PlayMusic()
    {
        if (_musicSource.isPlaying)
            StopMusic();

        _musicSource.clip = _musics[UserUtils.GetIntRandomNumber(0, _musics.Count)];
        _musicSource.Play();
    }

    private void StopMusic()
    {
        _musicSource.Stop();
    }

    private void OnGameStarted()
    {
        PlayMusic();
    }

    private void OnGameRestarted()
    {
        StopMusic();
        PlayMusic();
    }

    private void OnGameCompleted()
    {
        //play succes Sound;
        StopMusic();
    }

    private void OnGameContinued()
    {
        PlayMusic();
    }

    private void OnGameLoss()
    {
        //play loss sound;
        StopMusic();
    }

    private void OnGameLeaved()
    {
        //play loss sound;
        StopMusic();
    }

    private void OnSelectedSkin()
    {
        PlaySound(_skinSelectSound);
    }

    private void OnPurchasedSkin()
    {
        PlaySound(_skinBuySound);
    }
}
