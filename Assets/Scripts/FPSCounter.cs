using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private Text _fpsText;

    private int _fps;
    private float _timer = 0f;
    private int _frameCount = 0;

    private const float _updateInterval = 0.1f;

    void Update()
    {
        _frameCount++;
        _timer += Time.deltaTime;

        if (_timer >= _updateInterval)
        {
            _fps = Mathf.RoundToInt(_frameCount / _updateInterval);            
            _frameCount = 0;
            _timer -= _updateInterval;
        }

        _fpsText.text = $"FPS: {_fps}";
    }
}
