using UnityEngine;

namespace _Game.Core.Shapes
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        
        public void SetMaterial(Material material) => meshRenderer.sharedMaterial = material;
    }
}
