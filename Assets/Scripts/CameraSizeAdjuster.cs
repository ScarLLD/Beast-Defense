using UnityEngine;
using UnityEngine.UI;

public class CameraSizeAdjuster : MonoBehaviour
{
    [SerializeField] private CanvasScaler scaler;

    private float baseHeight;
    private float originalOrthographicSize;
    private Camera _camera;

    [SerializeField] private float maxSizeMultiplier = 1.15f; // Макс. увеличение 15%
    [SerializeField] private float minSizeMultiplier = 0.9f;  // Мин. уменьшение 10%
    [SerializeField] private AnimationCurve sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private void Start()
    {
        _camera = Camera.main;
        originalOrthographicSize = _camera.orthographicSize;

        if (scaler != null)
        {
            baseHeight = scaler.referenceResolution.y;
            Debug.Log($"Using CanvasScaler reference: {baseHeight}");
        }
        else
        {
            baseHeight = 1920f;
            Debug.LogWarning("CanvasScaler not assigned, using default 1920");
        }

        AdjustCameraSize();
    }

    private void Update()
    {
        if (Application.isEditor && (Screen.width != lastWidth || Screen.height != lastHeight))
        {
            AdjustCameraSize();
        }
    }

    private int lastWidth, lastHeight;

    private void AdjustCameraSize()
    {
        if (!_camera.orthographic || baseHeight == 0) return;

        float currentHeight = Screen.height;
        float heightRatio = currentHeight / baseHeight;

        // Нормализуем ratio для кривой (пример: 0.5 = 0, 1.0 = 0.5, 1.5 = 1.0)
        float normalizedRatio = Mathf.Clamp01((heightRatio - 0.5f) / 1.0f);

        // Получаем значение из кривой
        float curveValue = sizeCurve.Evaluate(normalizedRatio);

        // Применяем ограниченный множитель
        float sizeMultiplier = Mathf.Lerp(minSizeMultiplier, maxSizeMultiplier, curveValue);

        _camera.orthographicSize = originalOrthographicSize * sizeMultiplier;

        Debug.Log($"Resolution: {Screen.width}x{Screen.height}\n" +
                 $"Height ratio: {heightRatio:F2}\n" +
                 $"Normalized: {normalizedRatio:F2}\n" +
                 $"Multiplier: {sizeMultiplier:F2}\n" +
                 $"Final size: {_camera.orthographicSize:F2}");

        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}