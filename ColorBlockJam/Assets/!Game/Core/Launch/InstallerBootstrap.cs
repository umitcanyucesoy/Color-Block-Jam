using _Game.Core.Cameras;
using _Game.Core.Control;
using _Game.Core.Factory;
using _Game.Core.Grid;
using _Game.Core.Input;
using _Game.Core.Level;
using _Game.Core.Pool;
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
        [SerializeField] private PoolData poolData;
        [SerializeField] private ShapeData shapeData;

        [Header("Prefabs")]
        [SerializeField] private Shape shapePrefab;

        [Header("Controllers")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LevelController levelController;
        [SerializeField] private ShapeController shapeController;

        private IPoolService _poolService;
        private ICameraController _cameraController;
        private IInputService _inputService;
        private IGridService _gridService;
        private IShapeFactory _shapeFactory;
        private IShapeController _shapeController;
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
            _inputService?.Dispose();
            _shapeController?.Dispose();

            ServiceLocator.Unregister<ILevelController>();
            ServiceLocator.Unregister<IShapeFactory>();
            ServiceLocator.Unregister<IGridService>();
            ServiceLocator.Unregister<IInputService>();
            ServiceLocator.Unregister<ICameraController>();
            ServiceLocator.Unregister<IPoolService>();
        }

        private void InstallServices()
        {
            _levelController = levelController;
            _cameraController = cameraController;
            _shapeController = shapeController; 
            
            _poolService = new PoolService(poolData);
            ServiceLocator.Register(_poolService);

            _cameraController = cameraController;
            ServiceLocator.Register(_cameraController);

            _inputService = new InputService(_cameraController);
            ServiceLocator.Register(_inputService);

            _gridService = new GridService(gridData, _poolService);
            ServiceLocator.Register(_gridService);

            _shapeFactory = new ShapeFactory(shapePrefab, colorPalette, _gridService, _poolService, shapeData);
            ServiceLocator.Register(_shapeFactory);
            
            _levelController.Init(_gridService, _shapeFactory);
            ServiceLocator.Register(_levelController);
        }

        private void InstallGame()
        {
            _shapeController.Init(_gridService, shapeData);
            _levelController.LoadCurrent();
        }
    }
}
