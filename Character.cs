using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Tools.CompositeProperties;

namespace Larje.Character
{
	public class Character : MonoBehaviour
	{
		[SerializeField] private Type _characterType;
        [Space]
        [SerializeField] private BoolComposite _isActive = new BoolComposite();
        [SerializeField] private List<GameState> _activeInGameStates = new List<GameState>();

        [InjectService] private IGameStateService _gameStateService;
		
		private Health _health;
        private Dictionary<System.Type, CharacterAbility> _abilities = new Dictionary<System.Type, CharacterAbility>();
		
		public bool IsAlive => _health == null || _health.IsAlive;
        public bool IsActive => _isActive.Value && IsActiveInCurrentGameState();
        public Type CharacterType => _characterType; 
		public Health Health 
        {
            get
            {
                if (_health == null)
                {
                    _health = GetComponentInChildren<Health>();
                }
                return _health;
            }
        }

        public BoolComposite IsActiveComposite => _isActive;

		public event Action EventDeath;

        public T FindAbility<T>() where T : CharacterAbility
        {
            T ability = null;

            if (_abilities.ContainsKey(typeof(T)))
            {
                ability = _abilities[typeof(T)] != null ? _abilities[typeof(T)] as T : null;
            }
            else
            {
                ability = GetComponentInChildren<T>();
                _abilities.Add(typeof(T), ability);
            }

            return ability;
        }

		private void Awake()
		{
            DIContainer.InjectTo(this);

            if (Health != null)
            {
                Health.EventDeath += OnDeath;
            }
		}

		private void OnDeath(DamageData data)
		{
			EventDeath?.Invoke();
		}

        private bool IsActiveInCurrentGameState()
        {
            bool isActive = false;
            foreach (GameState gameState in _activeInGameStates)
            {
                if (_gameStateService.CurrentState.name == gameState.name)
                {
                    isActive = true;
                    break;
                }
            }
            return isActive;
        }
	}

	public enum Type
	{
		Player,
		NPC
	}
}
