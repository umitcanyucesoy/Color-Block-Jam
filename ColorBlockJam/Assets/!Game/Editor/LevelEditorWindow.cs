using System.IO;
using _Game.Data;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace _Game.Editor
{
    public class LevelEditorWindow : OdinMenuEditorWindow
    {
        private LevelDatabase _database;

        [MenuItem("ColorBlockJam/Level Editor")]
        private static void Open()
        {
            var window = GetWindow<LevelEditorWindow>();
            window.titleContent = new GUIContent("Level Editor");
            window.minSize = new Vector2(820, 600);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false);
            tree.Config.DrawSearchToolbar = true;

            _database = FindDatabase();
            if (!_database)
                return tree;

            tree.Add("Play Order", _database);

            var levels = _database.Levels;
            for (int i = 0; i < levels.Count; i++)
            {
                var level = levels[i];
                if (level)
                    tree.Add("Levels/" + i.ToString("00") + "  " + level.name, level);
            }

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();

            if (!_database)
            {
                if (SirenixEditorGUI.ToolbarButton("Create Level Database"))
                    CreateDatabase();
            }
            else
            {
                if (SirenixEditorGUI.ToolbarButton("New Level"))
                    CreateLevel();
                if (SirenixEditorGUI.ToolbarButton("Duplicate"))
                    DuplicateSelected();
                if (SirenixEditorGUI.ToolbarButton("Delete"))
                    DeleteSelected();
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private LevelData SelectedLevel => MenuTree?.Selection?.SelectedValue as LevelData;

        private static LevelDatabase FindDatabase()
        {
            var guids = AssetDatabase.FindAssets("t:" + nameof(LevelDatabase));
            if (guids.Length == 0)
                return null;

            return AssetDatabase.LoadAssetAtPath<LevelDatabase>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        private void CreateDatabase()
        {
            var db = CreateInstance<LevelDatabase>();
            AssetDatabase.CreateAsset(db, "Assets/LevelDatabase.asset");

            var guids = AssetDatabase.FindAssets("t:" + nameof(LevelData));
            for (int i = 0; i < guids.Length; i++)
            {
                var level = AssetDatabase.LoadAssetAtPath<LevelData>(AssetDatabase.GUIDToAssetPath(guids[i]));
                if (level)
                    db.EditorAdd(level);
            }

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            ForceMenuTreeRebuild();
        }

        private void CreateLevel()
        {
            var level = CreateInstance<LevelData>();
            var path = AssetDatabase.GenerateUniqueAssetPath(LevelsFolder() + "/Level_" + (_database.Count + 1).ToString("00") + ".asset");
            AssetDatabase.CreateAsset(level, path);

            Undo.RecordObject(_database, "Add Level");
            _database.EditorAdd(level);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            ForceMenuTreeRebuild();
            TrySelectMenuItemWithObject(level);
        }

        private void DuplicateSelected()
        {
            var level = SelectedLevel;
            if (!level)
                return;

            var src = AssetDatabase.GetAssetPath(level);
            var dst = AssetDatabase.GenerateUniqueAssetPath(src);
            if (!AssetDatabase.CopyAsset(src, dst))
                return;

            var copy = AssetDatabase.LoadAssetAtPath<LevelData>(dst);

            Undo.RecordObject(_database, "Duplicate Level");
            _database.EditorAdd(copy);
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();

            ForceMenuTreeRebuild();
            TrySelectMenuItemWithObject(copy);
        }

        private void DeleteSelected()
        {
            var level = SelectedLevel;
            if (!level)
                return;

            if (!EditorUtility.DisplayDialog("Delete Level", $"Delete '{level.name}'? This cannot be undone.", "Delete", "Cancel"))
                return;

            Undo.RecordObject(_database, "Remove Level");
            _database.EditorRemove(level);
            EditorUtility.SetDirty(_database);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(level));
            AssetDatabase.SaveAssets();

            ForceMenuTreeRebuild();
        }

        private string LevelsFolder()
        {
            var folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_database));
            return string.IsNullOrEmpty(folder) ? "Assets" : folder.Replace('\\', '/');
        }
    }
}
