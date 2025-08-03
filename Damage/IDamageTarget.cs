using UnityEngine;

public interface IDamageTarget
{
    GameObject gameObject { get; }
    void SendDamage(DamageData damageData);
}
