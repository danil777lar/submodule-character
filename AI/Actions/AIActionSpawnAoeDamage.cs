using System.Collections.Generic;
using System.Linq;
using Larje.Character.AI;
using UnityEngine;

public class AIActionSpawnAoeDamage : AIAction
{
    [Space]
    [SerializeField] private AoeDamageSource prefab;
    
    private List<IDamageTarget> selfDamagedTargets = new List<IDamageTarget>();

    protected override void OnInitialized()
    {
        selfDamagedTargets = Brain.Owner.GetComponentsInChildren<IDamageTarget>().ToList();
    }

    protected override void OnEnterState()
    {
        if (prefab != null)
        {
            AoeDamageSource instance = Instantiate(prefab, Brain.Owner.transform.parent, true);
            instance.SetBlackList(selfDamagedTargets);
            instance.transform.position = Brain.Owner.transform.position;
            instance.transform.rotation = Brain.Owner.transform.rotation;
        }
    }

    public override void PerformAction()
    {
        
    }
}
