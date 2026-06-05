using System;
using _Game.Core.Grid;
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

        private IGridService _gridService;

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
            ServiceLocator.Unregister<IGridService>();
        }

        private void InstallServices()
        {
            _gridService = new GridService(gridData, levelData);
            ServiceLocator.Register(_gridService);
        }

        private void InstallGame()
        {
            _gridService.Build();
        }
    }
}
