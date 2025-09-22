using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] protected PlayerHealth _playerHealth;

    protected void OnEnable()
    {
        _playerHealth.AmountChanged += OnAmountChanged;
    }

    protected void Start()
    {
        OnAmountChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
    }

    protected void OnDisable()
    {
        _playerHealth.AmountChanged -= OnAmountChanged;
    }

    public virtual void OnAmountChanged(float currentHealth, float maxHealth) { }
}
