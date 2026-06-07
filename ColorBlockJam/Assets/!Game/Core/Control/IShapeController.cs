using _Game.Core.Audio;
using _Game.Core.Grid;
using _Game.Core.Match;
using _Game.Data;
using _Game.Services;

namespace _Game.Core.Control
{
    public interface IShapeController
    {
        void Init(IGridService grid, IMatchController matchController, ISoundService sound);
        void Dispose();
    }
}
