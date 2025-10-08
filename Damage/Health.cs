using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageTarget
{
    [SerializeField] private float initialHealth;
    [SerializeField] private List<GameObject> spawnOnDamage;
    [SerializeField] private List<GameObject> spawnOnDeath;
    
    private float _currentHealth;
    
    public bool IsAlive => _currentHealth > 0;
    public float HealthPercent => (float)_currentHealth / (float)initialHealth;
    public float CurrenHealth => _currentHealth;
    
    public event Action<DamageData> EventDamage;
    public event Action<DamageData> EventDeath;

    public void SendDamage(DamageData damageData)
    {
        if (IsAlive)
        {
            damageData.damagedTargets ??= new List<IDamageTarget>();
            damageData.damagedTargets.Add(this);
            
            _currentHealth -= damageData.damage;
            SpawnEffects(spawnOnDamage, damageData);
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
        SpawnEffects(spawnOnDeath, damageData);
        EventDeath?.Invoke(damageData);
    }
    
    private void SpawnEffects(List<GameObject> prefabs, DamageData damageData)
    {
        foreach (GameObject prefab in prefabs)
        {
            GameObject go = Instantiate(prefab, transform.parent);
            if (go.TryGetComponent(out IDamageTarget target))
            {
                target.SendDamage(damageData);
            }
        }
    }
}
