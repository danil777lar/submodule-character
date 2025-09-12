using Larje.Character.AI;
using UnityEngine;

public class AIDecisionGrounded : AIDecision
{
    private bool _grounded;
    private CharacterPhysics _physics;
    
    public override bool Decide()
    {
        return _grounded;
    }
    
    protected override void OnInitialize()
    {
        _physics = Brain.Owner.GetComponent<CharacterPhysics>();   
    }

    protected override void OnEnterState()
    {
        _grounded = false;
        _physics.EventGrounded += OnGrounded;
    }

    protected override void OnExitState()
    {
        _physics.EventGrounded -= OnGrounded;
    }

    private void OnGrounded()
    {
        _grounded = true;
    }
}
