using _Game.Core.Factory;
using _Game.Core.Grid;
using _Game.Data;
using _Game.Services;

namespace _Game.Core.Level
{
    public interface ILevelController : IService
    {
        int Index { get; }
        LevelData Current { get; }

        void Init(IGridService gridService, IShapeFactory shapeFactory, IEnvironmentFactory environmentFactory);        void LoadCurrent();
        void NextLevel();
        void RetryLevel();
    }
}
