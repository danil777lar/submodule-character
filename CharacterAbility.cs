using UnityEngine;

namespace Larje.Character
{
    public class CharacterAbility : MonoBehaviour
    {
        [SerializeField] private bool abilityEnabled = true;
        
        public bool Initialized { get; private set; }
        public virtual bool Permitted => character.IsAlive && character.IsActive.Value && abilityEnabled;
        
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
