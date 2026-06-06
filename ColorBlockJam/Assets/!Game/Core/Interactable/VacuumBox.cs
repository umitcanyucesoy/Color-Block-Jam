using UnityEngine;

namespace _Game.Core.Interactable
{
    public class VacuumBox : InteractableBox
    {
        [SerializeField] private MeshRenderer meshRenderer;

        public void SetColor(Material material)
        {
            meshRenderer.sharedMaterial = material;
        }
    }
}