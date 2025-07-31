using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UsableItem : MonoBehaviour
{
    private Dictionary<int, bool> _actions = new Dictionary<int, bool>();
    
    public void StartAction(int actionId)
    {
        if (!_actions[actionId])
        {
            _actions[actionId] = true;
            OnActionStarted();
        }
    }

    public void StopAction(int actionId)
    {
        if (!_actions[actionId])
        {
            _actions[actionId] = false;
            OnActionStopped();
        }
    }

    public abstract bool CanStartAction(int actionId);
    
    protected abstract void OnActionStarted();
    protected abstract void OnActionStopped();
}
