using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Data
{
    [CreateAssetMenu(fileName = "LevelDatabase", menuName = "ColorBlockJam/Level Database", order = -10)]
    public class LevelDatabase : ScriptableObject
    {
        [ListDrawerSettings(DraggableItems = true, ShowFoldout = false)]
        [SerializeField] private List<LevelData> levels = new();

        public int Count => levels.Count;
        public LevelData Get(int index) => index >= 0 && index < levels.Count ? levels[index] : null;

#if UNITY_EDITOR
        public IReadOnlyList<LevelData> Levels => levels;

        public void EditorAdd(LevelData level)
        {
            if (level && !levels.Contains(level))
                levels.Add(level);
        }

        public void EditorRemove(LevelData level) => levels.Remove(level);
#endif
    }
}
