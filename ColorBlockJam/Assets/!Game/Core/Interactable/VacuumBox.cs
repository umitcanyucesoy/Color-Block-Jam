using _Game.Data;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Interactable
{
    public class VacuumBox : InteractableBox
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private VacuumBoxData data;

        public void SetColor(Material material)
        {
            meshRenderer.sharedMaterial = material;
        }

        public void AnimateSwallow()
        {
            if (!data) return;

            transform.DOKill(true);
            transform.DOPunchPosition(Vector3.down * data.punchDepth, data.punchDuration, data.punchVibrato, data.punchElasticity);
        }

        public override void OnDespawn()
        {
            transform.DOKill();
        }
    }
}
