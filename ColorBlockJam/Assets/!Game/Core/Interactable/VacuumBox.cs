using _Game.Data;
using DG.Tweening;
using UnityEngine;

namespace _Game.Core.Interactable
{
    public class VacuumBox : InteractableBox
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private VacuumBoxData data;

        private float _restY;
        private bool _restCaptured;

        public void SetColor(Material material)
        {
            meshRenderer.sharedMaterial = material;
        }

        public void AnimateSwallow(float holdDuration)
        {
            if (!data) return;

            if (!_restCaptured)
            {
                _restY = transform.localPosition.y;
                _restCaptured = true;
            }

            transform.DOKill();

            var popDelay = Mathf.Max(0f, holdDuration - data.dipDuration);
            transform.DOLocalMoveY(_restY - data.dipDepth, data.dipDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => transform.DOLocalMoveY(_restY, data.popDuration)
                    .SetEase(Ease.OutBack, data.popOvershoot)
                    .SetDelay(popDelay));
        }

        public override void OnSpawn()
        {
            _restCaptured = false;
        }

        public override void OnDespawn()
        {
            transform.DOKill();
        }
    }
}
