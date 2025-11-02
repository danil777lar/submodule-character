using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Larje.Core.Tools.CompositeProperties;

namespace Larje.Character
{
	public class Character : MonoBehaviour
	{
		[field: SerializeField] public Type CharacterType { get; private set; }
		public BoolComposite IsActive = new BoolComposite();
		
		private Health _health;
		
		public bool IsAlive => _health == null || _health.IsAlive;
		public Health Health => _health;

		public event Action EventDeath;

		private void Awake()
		{
			_health = GetComponentInChildren<Health>();
            if (_health != null)
            {
                _health.EventDeath += OnDeath;
            }
		}

		private void OnDeath(DamageData data)
		{
			EventDeath?.Invoke();
		}
	}

	public enum Type
	{
		Player,
		NPC
	}
}
