using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "DragSettings", menuName = "ColorBlockJam/Drag Settings", order = 6)]
    public class DragSettings : ScriptableObject
    {
        public float liftHeight = 0.4f;
        public float liftDuration = 0.1f;
        public float dropDuration = 0.12f;
    }
}
