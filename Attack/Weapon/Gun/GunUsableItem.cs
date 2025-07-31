using System;
using UnityEngine;
using UnityEngine.Events;

public class GunUsableItem : UsableItem
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform model;
    [Space] 
    [SerializeField] private Transform defaultPoint;
    [SerializeField] private Transform aimPoint;
    [Space]
    [SerializeField] private BulletProjectile projectilePrefab;
    [Space]
    [SerializeField] private UnityEvent onShoot;

    private Func<bool> _isAiming;

    public event Action EventShoot;

    public override bool CanStartAction(int actionId)
    {
        return true;
    }

    protected override void OnActionStarted()
    {
        if (CanShoot())
        {
            Instantiate(projectilePrefab).Init(shootPoint);
            EventShoot?.Invoke();
            onShoot.Invoke();
        }   
    }

    protected override void OnActionStopped()
    {
        
    }

    public bool CanShoot()
    {
        return gameObject.activeInHierarchy && enabled;
    }

    private void Update()
    {
        model.localPosition = Vector3.Lerp(model.localPosition, GetCurrentPoint().localPosition, Time.deltaTime * 10f);
    }

    private Transform GetCurrentPoint()
    {
        return _isAiming.Invoke() ? aimPoint : defaultPoint;
    }
}
