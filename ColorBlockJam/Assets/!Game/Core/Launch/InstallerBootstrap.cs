using _Game.Core.Factory;
using _Game.Core.Grid;
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
        [SerializeField] private LevelData levelData;
        [SerializeField] private ColorPalette colorPalette;

        [Header("Prefabs")]
        [SerializeField] private Shape shapePrefab;

        private IGridService _gridService;
        private IShapeFactory _shapeFactory;

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
            ServiceLocator.Unregister<IShapeFactory>();
            ServiceLocator.Unregister<IGridService>();
        }

        private void InstallServices()
        {
            _gridService = new GridService(gridData, levelData);
            ServiceLocator.Register(_gridService);

            _shapeFactory = new ShapeFactory(shapePrefab, colorPalette, _gridService);
            ServiceLocator.Register(_shapeFactory);
        }

        private void InstallGame()
        {
            _gridService.Build();
            _shapeFactory.BuildLevel(levelData);
        }
    }
}
