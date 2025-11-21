using Larje.Character;
using Larje.Character.AI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Character.AI
{
    public class AIBrainLoader : MonoBehaviour
    {
        public void SetBrain(string brainKey)
        {
            transform.DestroyAllChildren();
            Addressables.LoadAssetAsync<GameObject>(brainKey).Completed += handle =>
            {
                GameObject prefab = handle.Result;
                AIBrain brain = Instantiate(prefab, transform).GetComponent<AIBrain>();
                
                brain.ResetBrain();
            };
        }

        public void SetBrain(AIBrain brainPrefab)
        {
            transform.DestroyAllChildren();

            AIBrain brain = Instantiate(brainPrefab, transform);
            brain.ResetBrain();
        }
    }
}
