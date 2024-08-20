using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Internal.Editor
{
    public class SceneServicesScanner : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            var factories = FindObjects<SceneServicesFactory>();
            var behaviours = FindObjects<MonoBehaviour>();

            var services = new Dictionary<Scene, List<MonoBehaviour>>();

            foreach (var behaviour in behaviours)
            {
                if (behaviour is not ISceneService)
                    continue;

                var scene = behaviour.gameObject.scene;

                if (services.ContainsKey(scene) == false)
                    services[scene] = new List<MonoBehaviour>();

                services[scene].Add(behaviour);
            }

            foreach (var factory in factories)
            {
                if (services.TryGetValue(factory.gameObject.scene, out var targets) == false)
                    continue;

                factory.SetServices(targets.ToArray());
                EditorUtility.SetDirty(factory);
            }

            return paths;

            IReadOnlyList<T> FindObjects<T>() where T : Object
            {
                return Object.FindObjectsByType<T>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None);
            }
        }
    }
}