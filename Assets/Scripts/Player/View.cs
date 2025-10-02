using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerCube))]
public class View : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Shooter _shooter;

    private PlayerCube _cube;

    private void Awake()
    {
        _cube = GetComponent<PlayerCube>();
    }

    private void OnEnable()
    {
        _shooter.BulletsCountChanged += DisplayBullets;
    }

    private void OnDisable()
    {
        _shooter.BulletsCountChanged -= DisplayBullets;
    }

    public void DisplayBullets()
    {
        if (_cube.IsAvailable == true || _cube.HasClicked)
            _text.text = _shooter.BulletCount.ToString();
        else
            SetEmpty();
    }

    public void SetEmpty()
    {
        _text.text = string.Empty;
    }
}
