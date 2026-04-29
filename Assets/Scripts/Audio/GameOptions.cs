using UnityEngine;

public class GameOptions : MonoBehaviour
{
    [SerializeField] private AudioSource _soundAudio;
    [SerializeField] private AudioSource _musicAudio;

    [Header("Icons")]
    [SerializeField] private GameObject _musicNegativeIcon;
    [SerializeField] private GameObject _soundNegativeIcon;

    private const string SOUND_MUTED_KEY = "SoundMuted";
    private const string MUSIC_MUTED_KEY = "MusicMuted";

    private bool isSoundMuted = false;
    private bool isMusicMuted = false;

    private void Awake()
    {
        LoadSettings();
        ApplyAudioSettings();
    }

    private void LoadSettings()
    {
        isSoundMuted = PlayerPrefs.GetInt(SOUND_MUTED_KEY, 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTED_KEY, 0) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt(SOUND_MUTED_KEY, isSoundMuted ? 1 : 0);
        PlayerPrefs.SetInt(MUSIC_MUTED_KEY, isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyAudioSettings()
    {
        _soundAudio.mute = isSoundMuted;
        _musicAudio.mute = isMusicMuted;

        _soundNegativeIcon.SetActive(isSoundMuted);
        _musicNegativeIcon.SetActive(isMusicMuted);
    }

    public void ToggleSound()
    {
        isSoundMuted = !isSoundMuted;

        ApplyAudioSettings();
        SaveSettings();
    }

    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;

        ApplyAudioSettings();
        SaveSettings();
    }
}
