using _Game.Core.Audio;
using _Game.Core.Cameras;
using _Game.Core.Control;
using _Game.Core.Factory;
using _Game.Core.Flow;
using _Game.Core.Grid;
using _Game.Core.Input;
using _Game.Core.Interactable;
using _Game.Core.Level;
using _Game.Core.Match;
using _Game.Core.Pool;
using _Game.Core.Shapes;
using _Game.Core.UI;
using _Game.Data;
using _Game.Services;
using UnityEngine;

namespace _Game.Core.Launch
{
    public class InstallerBootstrap : MonoBehaviour
    {
        [Header("SO Injection")]
        [SerializeField] private GridData gridData;
        [SerializeField] private ColorPalette colorPalette;
        [SerializeField] private PoolData poolData;
        [SerializeField] private ShapeData shapeData;
        [SerializeField] private SoundData soundData;

        [Header("Component Injection")]
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LevelController levelController;
        [SerializeField] private ShapeController shapeController;
        [SerializeField] private MatchController matchController;
        [SerializeField] private VacuumBoxController vacuumBoxController;
        [SerializeField] private GameFlowController gameFlowController;
        [SerializeField] private UIController uiController;

        private IPoolService _poolService;
        private ICameraController _cameraController;
        private IInputService _inputService;
        private IGridService _gridService;
        private IShapeFactory _shapeFactory;
        private IEnvironmentFactory _environmentFactory;
        private IShapeController _shapeController;
        private ILevelController _levelController;
        private IMatchController _matchController;
        private IVacuumBoxController _vacuumBoxController;
        private IGameFlowController _gameFlowController;
        private IUIController _uiController;
        private ISoundService _soundService;

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
            ServiceLocator.Unregister<ISoundService>();
        }

        private void InstallServices()
        {
            _levelController = levelController;
            _cameraController = cameraController;
            _shapeController = shapeController;
            _matchController = matchController;
            _vacuumBoxController = vacuumBoxController;
            _gameFlowController = gameFlowController;
            _uiController = uiController;

            _poolService = new PoolService(poolData);
            ServiceLocator.Register(_poolService);

            _soundService = new SoundService(soundData);
            ServiceLocator.Register(_soundService);

            _cameraController = cameraController;
            ServiceLocator.Register(_cameraController);

            _inputService = new InputService(_cameraController);
            ServiceLocator.Register(_inputService);

            _gridService = new GridService(gridData, _poolService);
            ServiceLocator.Register(_gridService);
            
            _environmentFactory = new EnvironmentFactory(gridData, _poolService, colorPalette, _vacuumBoxController);
            ServiceLocator.Register(_environmentFactory);

            _shapeFactory = new ShapeFactory(colorPalette, _gridService, _poolService, shapeData);
            ServiceLocator.Register(_shapeFactory);
            
            _levelController.Init(_gridService, _shapeFactory, _environmentFactory);
            ServiceLocator.Register(_levelController);
        }

        private void InstallGame()
        {
            _matchController.Init(_gridService, _levelController, _poolService, _vacuumBoxController, _soundService);
            _shapeController.Init(_gridService, _matchController, _soundService);

            _gameFlowController.Init(_levelController, _shapeFactory, _soundService);
            _uiController.Init(_gameFlowController);
            _gameFlowController.StartLevel();
        }
    }
}
