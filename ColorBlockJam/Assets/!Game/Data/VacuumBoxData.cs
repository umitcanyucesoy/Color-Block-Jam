using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "VacuumBoxData", menuName = "ColorBlockJam/Vacuum Box Data", order = 7)]
    public class VacuumBoxData : ScriptableObject
    {
        [Header("Swallow Dip")]
        public float dipDepth = 0.2f;      
        public float dipDuration = 0.1f;   
        public float popDuration = 0.25f;  
        public float popOvershoot = 1.7f;  
    }
}
