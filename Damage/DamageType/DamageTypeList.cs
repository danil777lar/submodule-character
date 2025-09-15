using UnityEngine;

public partial class DamageType
{
    public static DamageType Physical => new DamageType("physical");
    public static DamageType Fire => new DamageType("fire");
}