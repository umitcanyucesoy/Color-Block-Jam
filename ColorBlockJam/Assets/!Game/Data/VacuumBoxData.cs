using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "VacuumBoxData", menuName = "ColorBlockJam/Vacuum Box Data", order = 7)]
    public class VacuumBoxData : ScriptableObject
    {
        [Header("Swallow Dip")]
        public float dipDepth = 0.2f;       // How far down (local Y) the box dips while swallowing.
        public float dipDuration = 0.1f;    // Speed of the downward dip on entry.
        public float popDuration = 0.25f;   // Speed of the pop back up after the shape is gone.
        public float popOvershoot = 1.7f;   // OutBack overshoot for the pop feel.
    }
}
