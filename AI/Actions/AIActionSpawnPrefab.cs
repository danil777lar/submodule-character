using Larje.Character.AI;
using UnityEngine;

public class AIActionSpawnPrefab : AIAction
{
    [Space]
    [SerializeField] private GameObject prefab;
    
    protected override void OnEnterState()
    {
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, Brain.Owner.transform.parent, true);
            instance.transform.position = Brain.Owner.transform.position;
            instance.transform.rotation = Brain.Owner.transform.rotation;
        }
    }

    public override void PerformAction()
    {
        
    }
}
