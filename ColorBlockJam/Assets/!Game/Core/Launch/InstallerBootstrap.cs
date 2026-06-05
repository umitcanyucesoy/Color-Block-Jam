using _Game.Core.Factory;
using _Game.Core.Grid;
using _Game.Core.Level;
using _Game.Core.Shapes;
using _Game.Data;
using _Game.Services;
using UnityEngine;

namespace _Game.Core.Launch
{
    public class InstallerBootstrap : MonoBehaviour
    {
        [Header("Configs")]
        [SerializeField] private GridData gridData;
        [SerializeField] private ColorPalette colorPalette;

        [Header("Prefabs")]
        [SerializeField] private Shape shapePrefab;

        [Header("Controllers")]
        [SerializeField] private LevelController levelController;

        private IGridService _gridService;
        private IShapeFactory _shapeFactory;
        private ILevelController _levelController;

        private void Awake()
        {
            InstallServices();
        }

        private void Start()
        {
            InstallGame();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ILevelController>();
            ServiceLocator.Unregister<IShapeFactory>();
            ServiceLocator.Unregister<IGridService>();
        }

        private void InstallServices()
        {
            _levelController = levelController;
            
            _gridService = new GridService(gridData);
            ServiceLocator.Register(_gridService);

            _shapeFactory = new ShapeFactory(shapePrefab, colorPalette, _gridService);
            ServiceLocator.Register(_shapeFactory);

            _levelController.Init(_gridService, _shapeFactory);
            ServiceLocator.Register(_levelController);
        }

        private void InstallGame()
        {
            _levelController.LoadCurrent();
        }
    }
}
