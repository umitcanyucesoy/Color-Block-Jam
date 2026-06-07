using _Game.Core.Grid;
using _Game.Core.Interactable;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "ColorBlockJam/Grid Config", order = 1)]
    public class GridData : ScriptableObject
    {
        [Header("References")]
        public Tile tilePrefab;
        public Transform edgeWallPrefab;
        public Transform cornerWallPrefab;
        public VacuumBox vacuumBoxPrefab;
        public Transform blockPrefab;

        [Header("Layout")]
        public float cellSize = 1f;
        public bool centered = true;
    }
}
