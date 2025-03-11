using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Internal
{
    [InlineEditor]
    public class AssetsStorage : ScriptableObject, IAssetsStorage
    {
        [SerializeField] private AssetsDictionary _assets;
        [SerializeField] private OptionsDictionary _options;

        public IReadOnlyDictionary<string, List<EnvAsset>> Assets => _assets;
        public IReadOnlyDictionary<PlatformType, OptionsRegistry> Options => _options;
        
        [Button]
        public void Scan()
        {
#if UNITY_EDITOR
            _assets.Clear();

            UnityEditor.AssetDatabase.Refresh();
            var all = GetAssets();
            var index = GetMaxIndex();
            var ids = new HashSet<int>();

            foreach (var asset in all)
            {
                try
                {
                    var key = GetKey();
                    
                    if (_assets.TryGetValue(key, out var collection) == false)
                    {
                        collection = new List<EnvAsset>();
                        _assets[key] = collection;
                    }

                    collection.Add(asset);
                    
                    if (asset is IEnvAssetValidator validator)
                        validator.OnValidation();

                    if (asset.Id == -1 || ids.Contains(asset.Id))
                    {
                        asset.SetId(index);
                        index++;
                    }
                    
                    ids.Add(asset.Id);

                    UnityEditor.EditorUtility.SetDirty(asset);

                    string GetKey()
                    {
                        if (asset is IEnvAssetKeyOverride keyOverride)
                            return keyOverride.GetKeyOverride();
                        
                        return asset.GetType().FullName!;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to process asset: {asset.name} : {e}");
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            IReadOnlyList<EnvAsset> GetAssets()
            {
                var items = UnityEditor.AssetDatabase.FindAssets("t:EnvAsset", new[] { "Assets/" }).ToArray();
                var assets = new List<EnvAsset>();

                foreach (var guid in items)
                {
                    var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<EnvAsset>(path);
                    assets.Add(asset);
                }

                return assets;
            }

            int GetMaxIndex()
            {
                var maxIndex = -1;

                foreach (var asset in all)
                {
                    if (asset.Id > maxIndex)
                        maxIndex = asset.Id;
                }

                return maxIndex;
            }
#endif
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        public static class StorageScanner
        {
            private static bool _isScanning;

            static StorageScanner()
            {
                ScanAssets();
            }
            
            [UnityEditor.MenuItem("Assets/Scan assets %w", priority = -1000)]
            public static void ScanAssets()
            {
                if (_isScanning == true)
                    return;

                var ids = UnityEditor.AssetDatabase.FindAssets("t:AssetsStorage");

                if (ids.Length == 0 || ids.Length > 1)
                    throw new Exception();

                _isScanning = true;

                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(ids[0]);
                var storage = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsStorage>(path);

                try
                {
                    storage.Scan();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                _isScanning = false;
                UnityEditor.EditorUtility.SetDirty(storage);
            }
        }
#endif
    }
}