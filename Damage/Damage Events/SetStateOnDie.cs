using Larje.Core;
using Larje.Core.Services;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class SetStateOnDie : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    
    [InjectService] private IGameStateService _gameStateService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        
        Health health = GetComponentInParent<Health>();
        health.EventDeath += OnDeath;
    }

    private void OnDeath(DamageData obj)
    {
        _gameStateService.SetGameState(gameState);
    }
}
