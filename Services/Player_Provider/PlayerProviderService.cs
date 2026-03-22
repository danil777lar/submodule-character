using Larje.Character;
using Larje.Core;
using UnityEngine;

namespace Larje.Character
{
    [BindService(typeof(IPlayerProviderService))]
    public class PlayerProviderService : Service, IPlayerProviderService
    {
        private Character _player;

        public override void Init()
        {
        }

        public bool TryGetPlayer(out Character player)
        {
            player = _player;
            return _player != null;
        }

        public void SetPlayer(Character player)
        {
            _player = player;
        }
    }
}
