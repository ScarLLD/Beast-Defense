using UnityEngine;
using UnityEngine.UI;

public class ScoreViewer : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _recordIdentifier;
    [SerializeField] private GameTimer _timer;
    [SerializeField] private ScoreReader _reader;

    private void OnEnable()
    {
        _timer.Stopped += OnTimerStopped;
    }

    private void OnDisable()
    {
        _timer.Stopped -= OnTimerStopped;
    }

    private void OnTimerStopped(float time)
    {
        int totalSeconds = (int)time;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        float fractionalPart = time - totalSeconds;

        int hundredthsOfSecond = (int)(fractionalPart * 100);

        string formattedTime = $"{minutes}:{seconds}.{hundredthsOfSecond}";

        Debug.Log(formattedTime);
        Display(formattedTime);

        if (_reader.GetScore > time)
            _recordIdentifier.SetActive(true);
        else
            _recordIdentifier.SetActive(false);
    }

    private void Display(string text)
    {
        _scoreText.text = text;
    }
}
