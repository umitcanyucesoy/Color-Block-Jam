using _Game.Data;
using _Game.Services;
using UnityEngine;

namespace _Game.Core.Grid
{
    public interface IGridService : IService
    {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }

        void Build(LevelData level);
        void Clear();

        bool IsInside(int x, int y);
        bool IsInside(Vector2Int coordinate);

        Tile GetTile(int x, int y);
        bool TryGetTile(int x, int y, out Tile tile);

        Vector3 CoordToWorld(int x, int y);
    }
}
