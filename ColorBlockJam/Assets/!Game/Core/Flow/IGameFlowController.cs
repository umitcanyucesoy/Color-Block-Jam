using _Game.Core.Audio;
using _Game.Core.Factory;
using _Game.Core.Level;
using _Game.Enums;

namespace _Game.Core.Flow
{
    public interface IGameFlowController
    {
        float TimeLeft { get; }
        int LevelIndex { get; }

        void Init(ILevelController levelController, IShapeFactory shapeFactory, ISoundService sound);
        void StartLevel();
        void NextLevel();
        void RetryLevel();
    }
}
