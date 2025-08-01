using System;
using Larje.Core;
using Larje.Core.Tools.CompositeProperties;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Larje.Character
{
    public class CharacterItemUser : CharacterAbility
    {
        [SerializeField] private Transform itemRoot;
        [SerializeField] private string overrideItemLayer;
        [SerializeField] private UsePositionOverride usePositionOverride = UsePositionOverride.None;

        private UsableItem _currentItem;

        public void StartAction(int actionId)
        {
            _currentItem?.StartAction(actionId);
        }

        public void StopAction(int actionId)
        {
            _currentItem?.StopAction(actionId);
        }
        
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

                if (usePositionOverride == UsePositionOverride.Camera)
                {
                    Camera cam = Camera.main;
                    _currentItem.SetUseOriginPosition(() => cam.transform.position);   
                    _currentItem.SetUseTargetPosition(() => cam.transform.position + cam.transform.forward * 10f);
                }
                
                if (!string.IsNullOrEmpty(overrideItemLayer))
                {
                    foreach (Transform child in _currentItem.GetComponentsInChildren<Transform>())
                    {
                        child.gameObject.layer = LayerMask.NameToLayer(overrideItemLayer);   
                    }
                }
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
        
        public enum UsePositionOverride
        {
            None,
            Camera
        }
    }
}