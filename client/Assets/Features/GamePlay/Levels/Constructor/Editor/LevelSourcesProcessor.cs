using System.IO;
using System.Linq;
using Services;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelSourcesProcessor : OdinEditorWindow
    {
        private const string SourcePath = "Assets/Features/GamePlay/Options/Levels/Source/";
        private const string DestinationBasePath = "Assets/Features/GamePlay/Options/Levels/";

        private const string
            LevelOptionsPath = "Assets/Features/GamePlay/Options/LevelOptionsTemplate.asset"; // Optional template

        private const string PrefabPath = "Assets/Prefabs/LevelPrefab.prefab"; // Change as needed

        [MenuItem("Tools/MyTool")]
        private static void OpenWindow()
        {
            GetWindow<LevelSourcesProcessor>().Show();
        }

        [SerializeField] private LevelSectionType _sectionType;
        [SerializeField] private Level _levelPrefab;

        [Button]
        private void Process()
        {
            if (!AssetDatabase.IsValidFolder(SourcePath))
            {
                Debug.LogError("Source path does not exist: " + SourcePath);
                return;
            }

            var sourceFiles = Directory.GetFiles(SourcePath, "*.asset")
                .Where(path => Path.GetFileName(path)[0] != '.')
                .ToList();

            for (var i = 0; i < sourceFiles.Count; i++)
            {
                var index = i + 1;
                var levelFolderName = $"Level_{index}";
                var destFolder = Path.Combine(DestinationBasePath, levelFolderName);

                if (!AssetDatabase.IsValidFolder(destFolder))
                    AssetDatabase.CreateFolder(DestinationBasePath, levelFolderName);

                var fileName = $"Level_{index}.asset";
                var sourceFilePath = sourceFiles[i];
                var destFilePath = Path.Combine(destFolder, fileName).Replace("\\", "/");

                // Move and rename original file
                AssetDatabase.CopyAsset(sourceFilePath, destFilePath);

                // Duplicate as _Source
                var sourceCopyPath = Path.Combine(destFolder, $"Level_{index}_Source.asset").Replace("\\", "/");
                AssetDatabase.CopyAsset(destFilePath, sourceCopyPath);

                // Create LevelOptions scriptable object
                var levelOptions = CreateInstance<LevelOptions>();
                var levelOptionsPath = Path.Combine(destFolder, fileName).Replace("\\", "/");
                AssetDatabase.CreateAsset(levelOptions, levelOptionsPath);

                // Instantiate prefab in scene or folder
                var prefabInstance = PrefabUtility.InstantiatePrefab(_levelPrefab) as Level;

                string prefabPath = Path.Combine(destFolder, $"Level_{index}_Prefab.prefab").Replace("\\", "/");
                PrefabUtility.SaveAsPrefabAsset(prefabInstance.gameObject, prefabPath);
                DestroyImmediate(prefabInstance.gameObject); // clean up

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Level processing completed.");
            }
        }
    }
}