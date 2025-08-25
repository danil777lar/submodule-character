using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GunUsableItem : UsableItem
{
    public const int ACTION_SHOOT = 0;
    public const int ACTION_AIM = 1;

    [SerializeField] private int shootCountPerAction = 1;
    [SerializeField] private int ammoCapacity = 8;
    [SerializeField] private float shootDelay = 0.5f;
    [SerializeField] private float equipDuration = 0.5f;
    [SerializeField] private float reloadDuration = 2f;
    [Space] 
    [SerializeField] private float minSpread = 0f;
    [SerializeField] private float maxSpread = 10f;
    [SerializeField] private float addSpreadOnShoot = 5f;
    [SerializeField] private float spreadDecreaseSpeed = 5f;
    [Space]
    [SerializeField] private Transform shootPoint;
    [Space]
    [SerializeField] private BulletProjectile projectilePrefab;
    [Space]
    [SerializeField] private UnityEvent onShoot;

    private bool _isShooting;
    private bool _isAiming;
    private int _currentShootCount;
    private int _currentAmmo;
    private float _spreadAngle = 0f;
    private float _equipTime = 0f;
    private float _unequipTime = 0f;
    private float _currentDelay;

    public bool IsShootInProgress => _currentDelay > 0f;
    public bool IsAiming => _isAiming;
    public int CurrentAmmo => _currentAmmo;
    public int TotalAmmo => ammoCapacity;
    public float SpreadAngle => _spreadAngle; 
    public float ShootProgress => 1f - (_currentDelay / shootDelay);
    public float EquipProgress => _equipTime;
    public float UnequipProgress => _unequipTime;
    public float ReloadProgress { get; private set; }


    public event Action EventShoot;

    public override void Equip()
    {
        DOVirtual.Float(0f, 1f, equipDuration, x => _equipTime = x);
    }

    public override void Unequip(Action onComplete)
    {
        DOVirtual.Float(0f, 1f, equipDuration, x => _unequipTime = x)
            .OnComplete(() => onComplete?.Invoke());
    }

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
        return gameObject.activeInHierarchy && enabled && EquipProgress >= 1f && UnequipProgress <= 0f && ReloadProgress <= 0f;
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

        if (actionId == ACTION_AIM)
        {
            _isAiming = true;
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

        if (actionId == ACTION_AIM)
        {
            _isAiming = false;
        }
    }

    private void Start()
    {
        _currentAmmo =  ammoCapacity;
    }

    private void Update()
    {
        UpdateReload();
        UpdateShoot();
        UpdateSpread();
    }

    private void UpdateReload()
    {
        if (!_isShooting && _currentAmmo <= 0)
        {
            Reload();
        }
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
    
    private void UpdateSpread()
    {
        if (_spreadAngle > minSpread)
        {
            _spreadAngle -= spreadDecreaseSpeed * Time.deltaTime;
            _spreadAngle = Mathf.Clamp(_spreadAngle, minSpread, maxSpread);
        }
    }

    private void AddSpread()
    {
        _spreadAngle += addSpreadOnShoot;
        _spreadAngle = Mathf.Clamp(_spreadAngle, minSpread, maxSpread);
    }

    private void Shoot()
    {
        if (CanShoot())
        {
            _currentAmmo--;
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
            spawnDirection = ApplySpread(spawnDirection,  _spreadAngle);
            
            Instantiate(projectilePrefab).Init(spawnPosition, spawnDirection, shootPoint.position);
            AddSpread();
            
            EventShoot?.Invoke();
            onShoot.Invoke();
        }   
    }
    
    private Vector3 ApplySpread(Vector3 direction, float halfAngleDeg)
    {
        direction = direction.normalized;
        
        float phi = Random.value * 2f * Mathf.PI;
        
        float cosA = Mathf.Cos(halfAngleDeg * Mathf.Deg2Rad);
        float cosT = Mathf.Lerp(1f, cosA, Random.value);
        float sinT = Mathf.Sqrt(1f - cosT * cosT);
        
        Vector3 tmp   = (Mathf.Abs(direction.y) < 0.999f) ? Vector3.up : Vector3.right;
        Vector3 right = Vector3.Cross(tmp, direction).normalized;
        Vector3 up    = Vector3.Cross(direction, right);

        return (right * (sinT * Mathf.Cos(phi)) +
                up    * (sinT * Mathf.Sin(phi)) +
                direction * cosT).normalized;
    }

    private void Reload()
    {
        if (CanShoot())
        {
            _isShooting  = true;
            DOVirtual.Float(0f, 1f, reloadDuration, x => ReloadProgress = x)
                .OnComplete(() =>
                {
                    _isShooting = false;
                    _currentAmmo = ammoCapacity;
                    ReloadProgress = 0f;
                });
        }
    }
}
