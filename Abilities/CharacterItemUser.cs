using System;
using Larje.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Larje.Character
{
    public class CharacterItemUser : CharacterAbility
    {
        [SerializeField] private Transform itemRoot;

        private UsableItem _currentItem;
        private Func<bool> _isAiming;

        private bool IsAiming => _isAiming != null && _isAiming();

        public void SetItem(string itemKey)
        {
            if (!string.IsNullOrEmpty(itemKey))
            {
                Addressables.LoadAssetAsync<GameObject>(itemKey).Completed += handle =>
                {
                    UsableItem itemPrefab = handle.Result.GetComponent<UsableItem>();
                    SetItem(itemPrefab);
                };
            }
        }

        public void SetItem(UsableItem itemPrefab)
        {
            RemoveItem();
            if (itemPrefab != null)
            {
                _currentItem = Instantiate(itemPrefab, itemRoot);
                _currentItem.transform.localPosition = Vector3.zero;
                _currentItem.transform.localRotation = Quaternion.identity;
            }
        }

        public void RemoveItem()
        {
            if (_currentItem != null)
            {
                Destroy(_currentItem.gameObject);
            }
        }

        private void Start()
        {
            DIContainer.InjectTo(this);
            RemoveItem();
        }
    }
}