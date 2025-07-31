using System;
using Larje.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Character
{
    public class CharacterAttack : CharacterAbility
    {
        [SerializeField] private Transform gunRoot;

        private Gun _currentGun;
        private Func<bool> _isAiming;

        private bool IsAiming => _isAiming != null && _isAiming();

        public void SetWeapon(string gunKey)
        {
            if (!string.IsNullOrEmpty(gunKey))
            {
                Addressables.LoadAssetAsync<GameObject>(gunKey).Completed += handle =>
                {
                    Gun gunPrefab = handle.Result.GetComponent<Gun>();
                    SetWeapon(gunPrefab);
                };
            }
        }

        public void SetWeapon(Gun gunPrefab)
        {
            RemoveGun();
            if (gunPrefab != null)
            {
                _currentGun = Instantiate(gunPrefab, gunRoot).Init(() => IsAiming);
                _currentGun.transform.localPosition = Vector3.zero;
                _currentGun.transform.localRotation = Quaternion.identity;
            }
        }

        public void RemoveGun()
        {
            if (_currentGun != null)
            {
                Destroy(_currentGun.gameObject);
            }
        }

        public void Attack()
        {
            if (_currentGun != null && CanShoot())
            {
                _currentGun.Shoot();
            }
        }

        public bool CanShoot()
        {
            return gameObject.activeInHierarchy && enabled;
        }

        public void SetAimCondition(Func<bool> isAiming)
        {
            _isAiming = isAiming;
        }

        private void Start()
        {
            DIContainer.InjectTo(this);
            RemoveGun();
        }
    }
}