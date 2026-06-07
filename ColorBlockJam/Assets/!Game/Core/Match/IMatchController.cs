using _Game.Core.Grid;
using _Game.Core.Interactable;
using _Game.Core.Level;
using _Game.Core.Pool;
using _Game.Core.Shapes;
using UnityEngine;

namespace _Game.Core.Match
{
    public interface IMatchController
    {
        void Init(IGridService grid, ILevelController levelController, IPoolService pool, IVacuumBoxController vacuums);
        bool IsDraggable(Shape shape, int x, int y);
        bool TrySwallow(Shape shape, Vector2 currentPos, Vector2 probePos);
    }
}