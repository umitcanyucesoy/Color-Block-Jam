using _Game.Core.Factory;
using _Game.Core.Grid;
using _Game.Data;
using UnityEngine;

namespace _Game.Core.Level
{
    public class LevelController : MonoBehaviour, ILevelController
    {
        [SerializeField] private LevelDatabase database;

        private IGridService _grid;
        private IShapeFactory _shapeFactory;
        private IEnvironmentFactory _envFactory;
        private int _index;

        public int Index => _index;
        public LevelData Current => database ? database.Get(_index) : null;

        public void Init(IGridService grid, IShapeFactory shapeFactory, IEnvironmentFactory envFactory)
        {
            _grid = grid;
            _shapeFactory = shapeFactory;
            _envFactory = envFactory;
        }

        public void LoadCurrent()
        {
            if (!database || database.Count == 0)
            {
                Debug.LogError("[LevelController] Level database is missing or empty.");
                return;
            }

            var level = database.Get(_index);

            _shapeFactory.Clear();
            _envFactory.Clear();
            _grid.Clear();

            _grid.Build(level);
            _envFactory.Build(level, _grid);
            _shapeFactory.BuildLevel(level);
        }

        public void NextLevel()
        {
            if (!database || database.Count == 0)
                return;

            _index = (_index + 1) % database.Count;
            LoadCurrent();
        }

        public void RetryLevel()
        {
            LoadCurrent();
        }
    }
}
