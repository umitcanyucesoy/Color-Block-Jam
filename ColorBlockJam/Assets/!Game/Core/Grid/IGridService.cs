using _Game.Core.Shapes;
using _Game.Data;
using _Game.Services;
using UnityEngine;

namespace _Game.Core.Grid
{
    public interface IGridService : IService
    {
        float CellSize { get; }

        void Build(LevelData level);
        void Clear();

        bool IsCellFree(int x, int y, Shape ignore);
        bool IsInside(int x, int y);
        void Occupy(Shape shape);
        void Free(Shape shape);

        Vector3 CoordToWorld(int x, int y);
        Vector3 CellToWorld(Vector2 cell);
        Vector2 WorldToCell(Vector3 world); 
    }
}
