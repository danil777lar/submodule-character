using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : MonoBehaviour
{
    private Func<Vector3> _useOriginPosition;  
    private Func<Vector3> _useTargetPosition;  
    private Dictionary<int, bool> _actions = new Dictionary<int, bool>();

    protected Func<Vector3> UseOriginPosition => _useOriginPosition;  
    protected Func<Vector3> UseTargetPosition => _useTargetPosition;

    public void StartAction(int actionId)
    {
        _actions[actionId] = true;
        OnActionStarted(actionId);
    }

    public void StopAction(int actionId)
    {
        _actions[actionId] = false;
        OnActionStopped(actionId);
    }
    
    public bool IsActionInProgress(int actionId)
    {
        return _actions.ContainsKey(actionId) && _actions[actionId];
    }
    
    public void SetUseOriginPosition(Func<Vector3> useOriginPosition)
    {
        _useOriginPosition = useOriginPosition;
    }
    
    public void SetUseTargetPosition(Func<Vector3> useTargetPosition)
    {
        _useTargetPosition = useTargetPosition;
    }

    public abstract bool CanStartAction(int actionId);

    protected virtual void OnActionStarted(int actionId) { }
    protected virtual void OnActionStopped(int actionId) { }
}
