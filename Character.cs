using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Larje.Core.Tools.CompositeProperties;
using Larje.Core;

namespace Larje.Character
{
	public class Character : MonoBehaviour
	{
		[SerializeField] private Type _characterType;
        [Space]
        [SerializeField] private BoolComposite _isActive = new BoolComposite();
        [SerializeField] private List<GameStateType> _activeInGameStates = new List<GameStateType>() { GameStateType.Playing };

        [InjectService] private IGameStateService _gameStateService;
		
		private Health _health;
		
		public bool IsAlive => _health == null || _health.IsAlive;
        public bool IsActive => _isActive.Value && _activeInGameStates.Contains(_gameStateService.CurrentState);
        public Type CharacterType => _characterType; 
		public Health Health => _health;

        public BoolComposite IsActiveComposite => _isActive;

		public event Action EventDeath;

		private void Awake()
		{
            DIContainer.InjectTo(this);

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
