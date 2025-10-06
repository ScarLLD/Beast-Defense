using UnityEngine;

public class GameOptions : MonoBehaviour
{
    [SerializeField] private GameObject _musicNegativeIcon;
    [SerializeField] private GameObject _soundNegativeIcon;
    [SerializeField] private GameObject _vibrationNegativeIcon;

    private bool isSound = true;
    private bool isMusic = true;
    private bool isVibration = true;

    public void ToggleSound()
    {
        isSound = !isSound;
        _soundNegativeIcon.SetActive(!isSound);
        AudioListener.volume = isSound ? 1 : 0;

    }

    public void ToggleMusic()
    {
        isMusic = !isMusic;
        _musicNegativeIcon.SetActive(!isMusic);

        int volume = isMusic ? 1 : 0;


    }

    public void ToggleVibration()
    {
        isVibration = !isVibration;
        _vibrationNegativeIcon.SetActive(!isVibration);
    }
}