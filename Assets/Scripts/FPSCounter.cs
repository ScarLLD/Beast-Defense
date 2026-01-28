using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private int _fps;
    private float _timer = 0f;
    private int _frameCount = 0;

    private const float _updateInterval = 0.5f; 

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
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(10, 10, 150, 40), "FPS: " + _fps, style);
    }
}
