using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyViewer : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        _wallet.CountChanged += DisplayCount;

        DisplayCount();
    }

    private void OnDisable()
    {
        _wallet.CountChanged -= DisplayCount;
    }

    private void DisplayCount()
    {
        _text.text = _wallet.GetMoneyCount().ToString();
    }
}
