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
        
        [HideInInspector, SerializeField]
        private ShapeColor[,] cellColors;

        [PropertyOrder(3), PropertySpace(SpaceBefore = 12), Title("Shapes")]
        [ListDrawerSettings(ShowFoldout = true, CustomAddFunction = nameof(AddNewShape))]
        [SerializeField]
        private List<ShapePlacement> shapes = new();

        public IReadOnlyList<ShapePlacement> Shapes => shapes;
        public int Width => cells?.GetLength(0) ?? 0;
        public int Height => cells?.GetLength(1) ?? 0;
        public CellType GetCell(int x, int y) => cells[x, y];
        public ShapeColor GetCellColor(int x, int y) => cellColors != null && x < Width && y < Height ? 
            cellColors[x, y] : ShapeColor.None;

        [Button(ButtonSizes.Medium), PropertyOrder(1), PropertySpace(SpaceBefore = 8)]
        private void ResizeMatrix()
        {
            var resizedCells = new CellType[width, height];
            var resizedColors = new ShapeColor[width, height]; 

            if (cells != null)
            {
                int copyX = Mathf.Min(width, cells.GetLength(0));
                int copyY = Mathf.Min(height, cells.GetLength(1));

                for (int x = 0; x < copyX; x++)
                for (int y = 0; y < copyY; y++)
                {
                    resizedCells[x, y] = cells[x, y];
                    if (cellColors != null)
                        resizedColors[x, y] = cellColors[x, y];
                }
            }

            cells = resizedCells;
            cellColors = resizedColors;
        }
        
        private void AddNewShape()
        {
            shapes ??= new List<ShapePlacement>();
            shapes.Add(new ShapePlacement 
            { 
                anchor = new Vector2Int(width / 2, height / 2) 
            });
        }

        [OnInspectorInit]
        private void EnsureMatrix()
        {
            cells ??= new CellType[width, height];
            cellColors ??= new ShapeColor[width, height];
        }
    }
}
