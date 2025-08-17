using System;
using UnityEngine;
using UnityEngine.Events;

public class GunUsableItem : UsableItem
{
    public const int ACTION_SHOOT = 0;
    public const int ACTION_AIM = 1;

    [SerializeField] private int shootCountPerAction = 1;
    [SerializeField] private float shootDelay = 0.5f;
    [Space]
    [SerializeField] private Transform shootPoint;
    [Space]
    [SerializeField] private BulletProjectile projectilePrefab;
    [Space]
    [SerializeField] private UnityEvent onShoot;
    
    private bool _isShooting;
    private int _currentShootCount;
    private float _currentDelay;
    
    public event Action EventShoot;

    public override bool CanStartAction(int actionId)
    {
        switch (actionId)
        {
            case ACTION_SHOOT:
                return !_isShooting && CanShoot();
            case ACTION_AIM:
                return true;
        }

        return true;
    }
    
    public bool CanShoot()
    {
        return gameObject.activeInHierarchy && enabled;
    }

    protected override void OnActionStarted(int actionId)
    {
        if (actionId == ACTION_SHOOT)
        {
            if (!_isShooting)
            {
                _isShooting = true;
                _currentDelay = 0f;
                _currentShootCount = shootCountPerAction;
            }
        }
    }
    
    protected override void OnActionStopped(int actionId)
    {
        if (actionId == ACTION_SHOOT)
        {
            if (_isShooting && shootCountPerAction < 0)
            {
                _isShooting = false;
            }
        }
    }

    private void Update()
    {
        UpdateShoot();
    }

    private void UpdateShoot()
    {
        if (_isShooting)
        {
            if (_currentDelay <= 0f)
            {
                if (_currentShootCount == 0)
                {
                    _isShooting = false;    
                }
                else
                {
                    _currentDelay = shootDelay;
                    _currentShootCount--;
                    Shoot();   
                }
            }
            else
            {
                _currentDelay -= Time.deltaTime;
            }
        }
    }

    private void Shoot()
    {
        if (CanShoot())
        {
            Vector3 spawnPosition = shootPoint.position;
            if (OriginPosition != null && UseOriginPosition != null && UseOriginPosition.Invoke())
            {
                spawnPosition = OriginPosition.Invoke();
            }
                
            Vector3 spawnDirection = shootPoint.forward;
            if (TargetPosition != null && UseOriginPosition != null && UseTargetPosition.Invoke())
            {
                spawnDirection = TargetPosition.Invoke() - spawnPosition;
            }

            Instantiate(projectilePrefab).Init(spawnPosition, spawnDirection, shootPoint.position);
            EventShoot?.Invoke();
            onShoot.Invoke();
        }   
    }
}
