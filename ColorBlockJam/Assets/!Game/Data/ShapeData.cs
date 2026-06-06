using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "ShapeData", menuName = "ColorBlockJam/Shape Data", order = 6)]
    public class ShapeData : ScriptableObject
    {
        [Header("Placement")]
        public float yOffset = 0.25f; 

        [Header("Drag")]
        public float liftHeight = 0.4f; 
        public float liftDuration = 0.1f;
        public float dropDuration = 0.12f;
        
        [Header("Swallow")]
        public float swallowDuration = 0.35f;
        public float swallowDepth = 0.5f; // Extra push into the vacuum box (along travel axis) so the shape tucks in instead of clipping/receding.
    }
}
