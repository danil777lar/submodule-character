using Larje.Character.AI;
using UnityEngine;

public class AIDecisionFarFromAnchor : AIDecision
{
    [SerializeField] private float distance;
    
    private IAIAnchorPosition _anchor;
    
    public override bool Decide()
    {
        return Vector3.Distance(Brain.Owner.transform.position, _anchor.Position) > distance;
    }
    
    protected override void OnInitialize()
    {
        _anchor = GetComponentInParent<IAIAnchorPosition>();
    }
}
