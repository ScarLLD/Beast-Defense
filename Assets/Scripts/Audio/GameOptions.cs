using UnityEngine;

public class GameOptions : MonoBehaviour
{
    [SerializeField] private AudioSource _soundAudio;
    [SerializeField] private AudioSource _musicAudio;

    [Header("Icons")]
    [SerializeField] private GameObject _musicNegativeIcon;
    [SerializeField] private GameObject _soundNegativeIcon;
    [SerializeField] private GameObject _vibrationNegativeIcon;

    private bool isSoundMuted = false;
    private bool isMusicMuted = false;
    private bool isVibrationDisabled = false;

    private void Awake()
    {
        _soundAudio.mute = isSoundMuted;
        _musicAudio.mute = isMusicMuted;
    }

    public void ToggleSound()
    {
        isSoundMuted = !isSoundMuted;
        _soundNegativeIcon.SetActive(isSoundMuted);

        _soundAudio.mute = isSoundMuted;
    }

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        _musicNegativeIcon.SetActive(isMusicMuted);

        _musicAudio.mute = isMusicMuted;
    }

    public void ToggleVibration()
    {
        isVibrationDisabled = !isVibrationDisabled;
        _vibrationNegativeIcon.SetActive(isVibrationDisabled);
    }
}