using System;
using UnityEngine;
using UnityEngine.Events;

public class BulletProjectile : Projectile
{
    [SerializeField] private float speed = 100f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float destroyDelay = 0.1f;
    [SerializeField] private LayerMask hitLayer;
    [Space] 
    [SerializeField] private UnityEvent onHit;

    private bool _destroying;
    private float _currentLifetime;
    
    public void Init(Transform shootPoint)
    {
        _currentLifetime = lifeTime;
        transform.position = shootPoint.position;
        transform.forward = shootPoint.forward;
    }

    private void FixedUpdate()
    {
        if (_destroying)
        {
            return;
        }
        
        Move();
    }

    private void Move()
    {
        bool hitten = false;
        Vector3 position = transform.position + transform.forward * (speed * Time.fixedDeltaTime);
        Vector3 direction = transform.forward;
        
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, speed * Time.fixedDeltaTime, hitLayer))
        {
            position = hit.point;
            direction = hit.normal; 
            hitten = true;
            
            IDamageTarget target = hit.collider.GetComponentInParent<IDamageTarget>();
            DamageData damageData = new DamageData
            {
                hitPoint = hit.point,
                hitNormal = hit.normal,
                hitDirection = transform.forward
            };
            target?.SendDamage(damageData);
        }
        
        transform.position = position;
        transform.forward = direction;
        _currentLifetime -= Time.fixedDeltaTime;

        if (hitten)
        {
            OnHit();
        }
        else if (_currentLifetime <= 0)
        {
            OnEndLifetime();
        }
    }

    private void OnHit()
    {
        onHit.Invoke();
        DestroySelf();
    }
    
    private void OnEndLifetime()
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        _destroying = true;
        Destroy(gameObject, destroyDelay);
    }
}
