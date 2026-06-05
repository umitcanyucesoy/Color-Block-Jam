namespace _Game.Core.Pool
{
    public interface IPoolable
    {
        void OnSpawn();
        void OnDespawn();
    }
}
