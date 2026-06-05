using TMPro;
using UnityEngine;
using YG;

public class SliderLevelViewer : MonoBehaviour
{
    [SerializeField] private LevelHolder _levelHolder;
    [SerializeField] private TMP_Text _sliderLevelViewer;

    private void Start()
    {
        DisplayText();
    }

    public void DisplayText()
    {
        _sliderLevelViewer.text = $"{YG2.saves.LevelNumber}";
    }
}
