using _Game.Core.Shapes;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ShapeData", menuName = "ColorBlockJam/Shape Data", order = 6)]
    public class ShapeData : ScriptableObject
    {
        [Header("Prefab")]
        public Shape shapePrefab;
        
        [Header("Placement")]
        public float yOffset = 0.25f; 

        [Header("Drag")]
        public float liftHeight = 0.4f; 
        public float liftDuration = 0.1f;
        public float dropDuration = 0.12f;
        
        [Header("Swallow")]
        public float swallowDuration = 0.35f;
    }
}
