using Larje.Character;
using Larje.Core;
using UnityEngine;

namespace Larje.Character
{
    public class PlayerProviderConnector : MonoBehaviour
    {
        [InjectService] private IPlayerProviderService _playerProviderService;

        private void Awake()
        {
            DIContainer.InjectTo(this);        

            Character player = GetComponentInParent<Character>();
            if (player != null)
            {
                _playerProviderService.SetPlayer(player);
            }
        }
    }
}
