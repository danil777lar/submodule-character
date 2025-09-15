using System.Collections.Generic;
using UnityEngine;

public struct DamageData
{
    public int damage;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public Vector3 hitDirection;
    
    public DamageType damageType;
    
    public List<IDamageTarget> damagedTargets;
}