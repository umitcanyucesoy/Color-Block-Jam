using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "VacuumBoxData", menuName = "ColorBlockJam/Vacuum Box Data", order = 7)]
    public class VacuumBoxData : ScriptableObject
    {
        [Header("Swallow Punch")]
        public float punchDepth = 0.2f;    
        public float punchDuration = 0.25f;
        public int punchVibrato = 1;       
        public float punchElasticity = 1f;
    }
}
