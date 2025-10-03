using System;
using Larje.Character;
using Larje.Character.AI;
using Larje.Core;
using Larje.Core.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIActionAttack : AIAction
{
    [SerializeField] private float hitChance = 0.2f;
    [SerializeField] private float shootRange = 5f;
    [Space]
    [SerializeField] private float shootDelayMin;
    [SerializeField] private float shootDelayMax;
    [Space]
    [SerializeField] private EntityId targetEntityId;

    private Character _targetCharacter;
    private CharacterPhysics _targetPhysics;
    
    private float _delay;
    private Vector3 _targetPosition;
    private CharacterItemUser _itemUser;

    public override void PerformAction()
    {
        if (_delay <= 0)
        {
            if (_targetCharacter.IsAlive)
            {
                UpdateTargetPosition();
                _itemUser.StartAction(0);
                _delay = Random.Range(shootDelayMin, shootDelayMax);
            }
        }
        else
        {
            _delay -= Time.deltaTime;
        }
    }
    
    protected override void OnInitialized()
    {
        DIContainer.InjectTo(this);

        _targetCharacter = DIContainer.GetEntityComponent<Character>(targetEntityId);
        _targetPhysics = DIContainer.GetEntityComponent<CharacterPhysics>(targetEntityId);
        
        if (_itemUser == null)
        {
            _itemUser = Brain.Owner.GetComponent<CharacterItemUser>();
        }
    }

    protected override void OnEnterState()
    {
        _itemUser.TargetPosition.AddValue(GetTargetPosition, () => 10);
    }
    
    protected override void OnExitState()
    {
        _itemUser.TargetPosition.RemoveValue(GetTargetPosition);
    }

    private void UpdateTargetPosition()
    {
        Vector3 playerCenter = _targetPhysics.transform.TransformPoint(_targetPhysics.ColliderCenter);
        
        bool mustHit = Random.value < hitChance;
        if (mustHit)
        {
            _targetPosition = playerCenter;
        }
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * shootRange;
            _targetPosition = playerCenter + randomDirection;
        }
    }

    private Vector3 GetTargetPosition()
    {
        return _targetPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_targetPosition, 0.25f);
    }
}
