using System;
using UnityEngine;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform model;
    [Space] 
    [SerializeField] private Transform defaultPoint;
    [SerializeField] private Transform aimPoint;
    [Space]
    [SerializeField] private Projectile projectilePrefab;
    [Space]
    [SerializeField] private UnityEvent onShoot;

    private Func<bool> _isAiming;

    public event Action EventShoot;
    
    public Gun Init(Func<bool> isAiming)
    {
        _isAiming = isAiming;
        return this;
    }
    
    public void Shoot()
    {
        if (CanShoot())
        {
            Instantiate(projectilePrefab).Init(shootPoint);
            EventShoot?.Invoke();
            onShoot.Invoke();
        }
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
