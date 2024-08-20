using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Internal
{
    [InlineEditor]
    public class AssetsStorage : ScriptableObject, IAssetsStorage
    {
        [SerializeField] private AssetsDictionary _assets;
        [SerializeField] private OptionsDictionary _options;

        public IReadOnlyDictionary<string, EnvAsset> Assets => _assets;
        public IReadOnlyDictionary<PlatformType, OptionsRegistry> Options => _options;

        [Button]
        public void Scan()
        {
#if UNITY_EDITOR
            _assets.Clear();

            AssetDatabase.Refresh();
            var items = AssetDatabase.FindAssets("t:EnvAsset", new[] { "Assets/" }).ToArray();

            var index = 1;
            
            foreach (var guid in items)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<EnvAsset>(path);

                try
                {
                    if (asset is IEnvAssetKeyOverride keyOverride)
                        _assets[keyOverride.GetKeyOverride()] = asset;
                    else
                        _assets[asset.GetType().FullName!] = asset;

                    if (asset is IEnvAssetValidator validator)
                        validator.OnValidation();

                    asset.SetId(index);
                    
                    EditorUtility.SetDirty(asset);
                    index++;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to process asset: {path} : {e}");
                }
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

#if UNITY_EDITOR
        public static class StorageScanner
        {
            private static bool _isScanning;

            [MenuItem("Assets/Scan assets %w", priority = -1000)]
            public static void ScanAssets()
            {
                if (_isScanning == true)
                    return ;

                var ids = AssetDatabase.FindAssets("t:AssetsStorage");

                if (ids.Length == 0 || ids.Length > 1)
                    throw new Exception();

                _isScanning = true;

                var path = AssetDatabase.GUIDToAssetPath(ids[0]);
                var storage = AssetDatabase.LoadAssetAtPath<AssetsStorage>(path);

                try
                {
                    storage.Scan();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    // ignored
                }

                _isScanning = false;
            }
        }
#endif
    }
}