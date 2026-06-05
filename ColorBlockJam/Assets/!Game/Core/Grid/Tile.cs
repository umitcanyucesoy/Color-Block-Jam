using UnityEngine;

namespace _Game.Core.Grid
{
    public class Tile : MonoBehaviour
    {
        private const string NamePrefix = "Tile_";

        public Vector2Int Coordinate { get; private set; }

        public void Setup(Vector2Int coordinate, Vector3 localPosition)
        {
            Coordinate = coordinate;
            transform.localPosition = localPosition;
            gameObject.name = $"{NamePrefix}{coordinate.x}_{coordinate.y}";
        }
    }
}
