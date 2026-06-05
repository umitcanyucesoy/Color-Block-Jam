using UnityEngine;

namespace _Game.Core.Shapes
{
    public class PolyominoShape : Shape
    {
        protected override void Setup(Material material, float cellSize)
        {
            var offsets = Definition.Offsets;
            units = new Unit[offsets.Count];

            for (int i = 0; i < offsets.Count; i++)
            {
                var unit = pool.Get(unitPrefab, transform);
                unit.transform.localPosition = new Vector3(offsets[i].x * cellSize, 0f, -offsets[i].y * cellSize);
                unit.SetMaterial(material);
                units[i] = unit;
            }
        }
    }
}
