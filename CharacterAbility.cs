using UnityEngine;

namespace Larje.Character
{
    public class CharacterAbility : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        
        protected Character character { get; private set; }
        protected CharacterPhysics physics { get; private set; }
        

        private void Start()
        {
            character = GetComponentInParent<Character>();
            physics = GetComponentInParent<CharacterPhysics>();

            Initialized = true;
            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
            
        }
    }
}
