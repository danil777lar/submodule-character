using Larje.Character.AI;
using UnityEngine;

public class AIActionSetBrain : AIAction
{
    [Space] 
    [SerializeField] private string brainKey;

    protected override void OnEnterState()
    {
        AIBrainLoader brainLoader = GetComponentInParent<AIBrainLoader>();
        if (brainLoader != null)
        {
            brainLoader.SetBrain(brainKey);
        }
    }

    public override void PerformAction()
    {
        
    }
}
