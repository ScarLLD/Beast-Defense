using UnityEngine;
using YG;

public class GameOptions : MonoBehaviour
{
    [SerializeField] private AudioSource _soundAudio;
    [SerializeField] private AudioSource _musicAudio;

    [Header("Icons")]
    [SerializeField] private GameObject _musicNegativeIcon;
    [SerializeField] private GameObject _soundNegativeIcon;

    private void Awake()
    {
        ApplyAudioSettings();
    }

    private void ApplyAudioSettings()
    {
        _soundAudio.mute = YG2.saves.SoundMuted;
        _musicAudio.mute = YG2.saves.MusicMuted;

        _soundNegativeIcon.SetActive(YG2.saves.SoundMuted);
        _musicNegativeIcon.SetActive(YG2.saves.MusicMuted);
    }

    public void ToggleSound()
    {
        YG2.saves.SoundMuted = !YG2.saves.SoundMuted;
        ApplyAudioSettings();
        YG2.SaveProgress();
    }

    public void ToggleMusic()
    {
        YG2.saves.MusicMuted = !YG2.saves.MusicMuted;
        ApplyAudioSettings();
        YG2.SaveProgress();
    }
}
