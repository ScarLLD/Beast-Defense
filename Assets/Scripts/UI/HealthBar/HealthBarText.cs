using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]

public class HealthBarText : HealthBar
{    
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public override void OnAmountChanged(float currentHealth, float maxHealth)
    {
        _text.SetText($"{currentHealth} / {maxHealth}");
    }
}
