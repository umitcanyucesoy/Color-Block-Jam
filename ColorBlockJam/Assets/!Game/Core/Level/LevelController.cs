using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Core.Grid;
using _Game.Data;
using UnityEngine;

namespace _Game.Core.Level
{
    public class LevelController : MonoBehaviour, ILevelController
    {
        [SerializeField] private List<LevelData> levels = new();

        private IGridService _grid;
        private IShapeFactory _shapeFactory;
        private int _index;

        public int Index => _index;
        public LevelData Current => _index >= 0 && _index < levels.Count ? levels[_index] : null;

        public void Init(IGridService grid, IShapeFactory shapeFactory)
        {
            _grid = grid;
            _shapeFactory = shapeFactory;
        }

        public void LoadCurrent()
        {
            if (levels.Count == 0)
            {
                Debug.LogError("[LevelController] Level list is empty.");
                return;
            }

            var level = levels[_index];

            _shapeFactory.Clear();
            _grid.Clear();

            _grid.Build(level);
            _shapeFactory.BuildLevel(level);
        }

        public void NextLevel()
        {
            if (levels.Count == 0)
                return;

            _index = (_index + 1) % levels.Count;
            LoadCurrent();
        }

        public void RetryLevel()
        {
            LoadCurrent();
        }
    }
}
