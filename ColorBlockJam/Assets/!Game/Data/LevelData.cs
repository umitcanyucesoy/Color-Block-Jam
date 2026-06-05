using System.Collections.Generic;
using _Game.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ColorBlockJam/Level Data", order = 0)]
    public partial class LevelData : SerializedScriptableObject
    {
        [BoxGroup("Dimensions"), MinValue(1), SerializeField]
        private int width = 5;

        [BoxGroup("Dimensions"), MinValue(1), SerializeField]
        private int height = 5;
        
        [PropertyOrder(2), PropertySpace(SpaceBefore = 8)]
        [TableMatrix(SquareCells = true, DrawElementMethod = "DrawCell", HorizontalTitle = "X", VerticalTitle = "Y")]
        [SerializeField]
        private CellType[,] cells;

        [PropertyOrder(3), PropertySpace(SpaceBefore = 12), Title("Shapes")]
        [ListDrawerSettings(ShowFoldout = true)]
        [SerializeField]
        private List<ShapePlacement> shapes = new();

        public IReadOnlyList<ShapePlacement> Shapes => shapes;
        public int Width => cells?.GetLength(0) ?? 0;
        public int Height => cells?.GetLength(1) ?? 0;
        public CellType GetCell(int x, int y) => cells[x, y];

        [Button(ButtonSizes.Medium), PropertyOrder(1), PropertySpace(SpaceBefore = 8)]
        private void ResizeMatrix()
        {
            var resized = new CellType[width, height];

            if (cells != null)
            {
                int copyX = Mathf.Min(width, cells.GetLength(0));
                int copyY = Mathf.Min(height, cells.GetLength(1));

                for (int x = 0; x < copyX; x++)
                for (int y = 0; y < copyY; y++)
                    resized[x, y] = cells[x, y];
            }

            cells = resized;
        }

        [OnInspectorInit]
        private void EnsureMatrix()
        {
            cells ??= new CellType[width, height];
        }
    }
}
