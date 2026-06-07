using _Game.Enums;
using _Game.Services;

namespace _Game.Core.Audio
{
    public interface ISoundService : IService
    {
        void Play(SoundId id);
    }
}
