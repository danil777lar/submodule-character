using Larje.Character;
using UnityEngine;

namespace Larje.Character
{
    public interface IPlayerProviderService
    {
        public bool TryGetPlayer(out Character player);
        public void SetPlayer(Character player);
    }
}
