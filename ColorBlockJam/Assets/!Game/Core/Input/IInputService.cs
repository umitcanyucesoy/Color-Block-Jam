using _Game.Services;

namespace _Game.Core.Input
{
    public interface IInputService : IService
    {
        void SetEnabled(bool enabled);
        void Dispose();
    }
}
