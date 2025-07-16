using TMPro;
using UnityEngine;

public class View : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Shooter _shooter;

    private void OnEnable()
    {
        _shooter.BulletsDecreased += DisplayBullets;
    }

    private void OnDisable()
    {
        _shooter.BulletsDecreased -= DisplayBullets;
    }

    public void DisplayBullets()
    {
        _text.text = _shooter.BulletCount.ToString();
    }
}
