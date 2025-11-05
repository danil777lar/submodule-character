using UnityEngine;

public interface IHealthDamagePreprocessor
{
    public DamageData ProcessDamage(DamageData damageData);
}
