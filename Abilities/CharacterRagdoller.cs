using UnityEngine;

namespace Larje.Character
{
    public class CharacterRagdoller : CharacterAbility
    {
        [SerializeField] private GameObject ragdollRoot;

        protected override void OnInitialized()
        {
            DisableRagdoll();
        }

        public void EnableRagdoll()
        {
            SetActiveComponents(true);
        }

        public void DisableRagdoll()
        {
            SetActiveComponents(false);
        }

        private void SetActiveComponents(bool active)
        {
            foreach (Rigidbody rb in ragdollRoot.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = !active;
            }

            foreach (Collider col in ragdollRoot.GetComponentsInChildren<Collider>())
            {
                col.enabled = active;
            }
        }
    }
}
