using _Game.Core.Pool;
using UnityEngine;

namespace _Game.Core.Interactable
{
    public abstract class InteractableBox : MonoBehaviour, IPoolable
    {
        public virtual void OnSpawn()
        {
            
        }

        public virtual void OnDespawn()
        {
            
        }
    }
}