using Larje.Character.AI;
using UnityEngine;
using UnityEngine.AI;

public class AIActionSetTargetFromAnchor : AIAction
{
    private IAIAnchorPosition _anchor;
    private Transform _target;
    
    public override void PerformAction()
    {
        
    }
    
    protected override void OnInitialized()
    {
        _anchor = GetComponentInParent<IAIAnchorPosition>();
        
        if (_target == null)
        {
            _target = new UnityEngine.GameObject("Walk Target").transform;
            _target.SetParent(transform);
        }
    }

    protected override void OnEnterState()
    {
        Brain.Target = _target;
    }

    protected override void OnExitState()
    {
    }

    private void Update()
    {
        if (ActionInProgress && _anchor != null)
        {
            _target.position = _anchor.Position;
        }
        else
        {
            _target.position = Brain.Owner.transform.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (_target == null)
        {
            return;
        }
        
        Gizmos.color = Color.green.SetAlpha(ActionInProgress ? 1f : 0.2f);
        Gizmos.DrawSphere(_target.position, 0.5f);
    }
}
