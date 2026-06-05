using _Game.Core.Grid;
using _Game.Data;
using _Game.Services;

namespace _Game.Core.Control
{
    public interface IShapeController
    {
        void Init(IGridService grid, DragSettings settings);
        void Dispose();
    }
}
