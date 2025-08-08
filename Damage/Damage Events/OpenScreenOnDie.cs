using Larje.Core;
using Larje.Core.Services.UI;
using ProjectConstants;
using UnityEngine;

public class OpenScreenOnDie : MonoBehaviour
{
    [SerializeField] private UIScreenType screenType;
    
    [InjectService] private UIService _uiService;
    
    private void Start()
    {
        DIContainer.InjectTo(this);
        
        Health health = GetComponent<Health>();
        health.EventDeath += OnDeath;
    }

    private void OnDeath(DamageData obj)
    {
        _uiService.GetProcessor<UIScreenProcessor>()
            .OpenScreen(new UIScreen.Args(screenType));
    }
}
