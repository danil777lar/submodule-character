using Larje.Character.AI;
using UnityEngine;

public class AIDecisionCloseToTarget : AIDecision
{
    [SerializeField] private float distance = 2f;

    public override bool Decide()
    {
        if (Brain.Target == null)
        {
            return false;
        }

        return Vector3.Distance(Brain.Owner.transform.position, Brain.Target.position) <= distance;
    }
}
