using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageTarget
{
    [SerializeField] private int initialHealth;
    [Space] 
    [SerializeField] private UnityEvent onDeath;
    
    private int _currentHealth;
    
    public bool IsAlive => _currentHealth > 0;
    public float HealthPercent => (float)_currentHealth / (float)initialHealth;
    
    public event Action<DamageData> EventDamage;
    public event Action<DamageData> EventDeath;

    public void SendDamage(DamageData damageData)
    {
        if (IsAlive)
        {
            _currentHealth -= damageData.damage;
            EventDamage?.Invoke(damageData);
            if (_currentHealth <= 0)
            {
                Death(damageData);
            }
        }
    }
    
    private void Start()
    {
        _currentHealth = initialHealth;
    }

    private void Death(DamageData damageData)
    {
        onDeath?.Invoke();
        EventDeath?.Invoke(damageData);
    }
}
