using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Interactable
{
    public interface IVacuumBoxController
    {
        void Register(Vector2Int vacuumCell, VacuumBox box, ShapeColor color);
        void BuildGroups();
        void PlaySwallow(Vector2Int vacuumCell, float holdDuration);
        void Clear();
    }
}
