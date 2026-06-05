using _Game.Core.Grid;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "ColorBlockJam/Grid Config", order = 1)]
    public class GridData : ScriptableObject
    {
        [Header("References")]
        public Tile tilePrefab;

        [Header("Layout")]
        public float cellSize = 1f;
        public bool centered = true;
    }
}
