using _Game.Core.Grid;
using _Game.Data;
using _Game.Services;

namespace _Game.Core.Factory
{
    public interface IEnvironmentFactory : IService
    {
        void Build(LevelData level, IGridService grid);
        void Clear();
    }
}