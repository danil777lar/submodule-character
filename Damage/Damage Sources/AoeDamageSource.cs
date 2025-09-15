using System.Collections.Generic;
using UnityEngine;

public abstract class AoeDamageSource : MonoBehaviour 
{
    private List<IDamageTarget> targetBlackList;
    
    protected IReadOnlyCollection<IDamageTarget> TargetBlackList => targetBlackList;
    
    public void SetBlackList(List<IDamageTarget> blackList)
    {
        targetBlackList = blackList;
    }
}
