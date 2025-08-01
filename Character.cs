using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace Larje.Character
{
	public class Character : MonoBehaviour
	{
		[field: SerializeField] public Type CharacterType { get; private set; }
		
		private Health _health;
		
		public bool IsAlive => _health == null || _health.IsAlive;

		private void Awake()
		{
			_health = GetComponentInChildren<Health>();
		}
	}

	public enum Type
	{
		Player,
		NPC
	}
}