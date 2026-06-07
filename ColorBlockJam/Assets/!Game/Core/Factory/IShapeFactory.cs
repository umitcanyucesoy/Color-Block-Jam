using _Game.Data;
using _Game.Services;

namespace _Game.Core.Factory
{
    public interface IShapeFactory : IService
    {
        int ActiveCount { get; }

        void BuildLevel(LevelData level);
        void Clear();
    }
}
