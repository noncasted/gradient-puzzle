using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Features.GamePlay
{
    public class AreasBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            
            foreach (var prefabGuid in prefabGuids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefab.TryGetComponent(out LevelConstructor constructor) == false)
                    continue;

                Object.DestroyImmediate(constructor, true);
            }
        }
    }
}