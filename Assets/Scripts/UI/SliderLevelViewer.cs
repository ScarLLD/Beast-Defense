using TMPro;
using UnityEngine;

public class SliderLevelViewer : MonoBehaviour
{
    [SerializeField] private LevelHolder _levelHolder;
    [SerializeField] private TMP_Text _sliderLevelViewer;

    private void OnEnable()
    {
        _levelHolder.LevelChanged += DisplayText;
    }

    private void OnDisable()
    {
        _levelHolder.LevelChanged -= DisplayText;
    }

    private void Start()
    {
        DisplayText();
    }

    private void DisplayText()
    {
        _sliderLevelViewer.text = $"Уровень {_levelHolder.GetLevelNumber}";
    }
}
