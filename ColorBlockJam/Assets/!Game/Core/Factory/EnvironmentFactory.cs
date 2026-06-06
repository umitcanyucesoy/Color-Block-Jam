using System.Collections.Generic;
using _Game.Core.Grid;
using _Game.Core.Interactable;
using _Game.Core.Pool;
using _Game.Data;
using _Game.Enums;
using UnityEngine;

namespace _Game.Core.Factory
{
    public class EnvironmentFactory : IEnvironmentFactory
    {
        private const string RootName = "[EnvironmentRoot]";

        private readonly GridData _data;
        private readonly IPoolService _pool;
        private readonly ColorPalette _palette;
        private readonly List<Transform> _edgeWalls = new();
        private readonly List<InteractableBox> _mechanics = new();
        
        private Transform _root;

        public EnvironmentFactory(GridData data, IPoolService pool, ColorPalette palette)
        {
            _data = data;
            _pool = pool;
            _palette = palette;
        }

        public void Build(LevelData level, IGridService grid)
        {
            Clear();

            if (!_root)
                _root = new GameObject(RootName).transform;

            int width = level.Width;
            int height = level.Height;

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var cellType = level.GetCell(x, y);
                
                if (cellType == CellType.Empty || cellType == CellType.Wall || cellType == CellType.VacuumBox)
                    continue;

                CellType topCell = grid.IsInside(x, y - 1) ? level.GetCell(x, y - 1) : CellType.Empty;
                CellType bottomCell = grid.IsInside(x, y + 1) ? level.GetCell(x, y + 1) : CellType.Empty;
                CellType leftCell = grid.IsInside(x - 1, y) ? level.GetCell(x - 1, y) : CellType.Empty;
                CellType rightCell = grid.IsInside(x + 1, y) ? level.GetCell(x + 1, y) : CellType.Empty;

                bool topBoundary = topCell == CellType.Wall || topCell == CellType.VacuumBox;
                bool bottomBoundary = bottomCell == CellType.Wall || bottomCell == CellType.VacuumBox;
                bool leftBoundary = leftCell == CellType.Wall || leftCell == CellType.VacuumBox;
                bool rightBoundary = rightCell == CellType.Wall || rightCell == CellType.VacuumBox;

                if (topCell == CellType.Wall) CreateEdgeWall(x, y, 0f, grid);
                else if (topCell == CellType.VacuumBox) CreateVacuumBox(x, y, 0f, grid, level.GetCellColor(x, y - 1));

                if (bottomCell == CellType.Wall) CreateEdgeWall(x, y, 180f, grid);
                else if (bottomCell == CellType.VacuumBox) CreateVacuumBox(x, y, 180f, grid, level.GetCellColor(x, y + 1));

                if (leftCell == CellType.Wall) CreateEdgeWall(x, y, -90f, grid);
                else if (leftCell == CellType.VacuumBox) CreateVacuumBox(x, y, -90f, grid, level.GetCellColor(x - 1, y));

                if (rightCell == CellType.Wall) CreateEdgeWall(x, y, 90f, grid);
                else if (rightCell == CellType.VacuumBox) CreateVacuumBox(x, y, 90f, grid, level.GetCellColor(x + 1, y));
                
                if (topBoundary && leftBoundary) CreateCornerWall(x, y, 0f, grid);
                if (topBoundary && rightBoundary) CreateCornerWall(x, y, 90f, grid);
                if (bottomBoundary && rightBoundary) CreateCornerWall(x, y, 180f, grid);
                if (bottomBoundary && leftBoundary) CreateCornerWall(x, y, -90f, grid);
            }
        }

        private void CreateEdgeWall(int x, int y, float yRotation, IGridService grid)
        {
            if (!_data.edgeWallPrefab) return;
            var wall = _pool.Get(_data.edgeWallPrefab, _root);
            wall.position = grid.CoordToWorld(x, y);
            wall.rotation = Quaternion.Euler(0f, yRotation, 0f);
            _edgeWalls.Add(wall);
        }

        private void CreateCornerWall(int x, int y, float yRotation, IGridService grid)
        {
            if (!_data.cornerWallPrefab) return;
            var wall = _pool.Get(_data.cornerWallPrefab, _root);
            wall.position = grid.CoordToWorld(x, y);
            wall.rotation = Quaternion.Euler(0f, yRotation, 0f);
            _edgeWalls.Add(wall);
        }

        private void CreateVacuumBox(int x, int y, float yRotation, IGridService grid, ShapeColor color)
        {
            if (!_data.vacuumBoxPrefab) return;
            var vacuum = _pool.Get(_data.vacuumBoxPrefab, _root);
            vacuum.transform.position = grid.CoordToWorld(x, y);
            vacuum.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            vacuum.SetColor(_palette.GetMaterial(color));
                
            _mechanics.Add(vacuum);
        }

        public void Clear()
        {
            for (int i = 0; i < _edgeWalls.Count; i++)
                if (_edgeWalls[i]) _pool.Release(_edgeWalls[i]);
            _edgeWalls.Clear();

            for (int i = 0; i < _mechanics.Count; i++)
                if (_mechanics[i]) _pool.Release(_mechanics[i]);
            _mechanics.Clear();
        }
    }
}