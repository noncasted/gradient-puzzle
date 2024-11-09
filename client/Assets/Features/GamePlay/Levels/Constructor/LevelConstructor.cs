using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class LevelConstructor : MonoBehaviour
    {
        [FoldoutGroup("Options")] [SerializeField]
        private int _simplifyIterations = 1;

        [FoldoutGroup("Options")] [SerializeField]
        private float _distanceThreshold = 1f;

        [FoldoutGroup("Options")] [SerializeField]
        private int _erosionPixels;

        [FoldoutGroup("Options")] [SerializeField]
        private byte _colorEpsilon = 10;

        [SerializeField] private Level _level;
        [SerializeField] private Area _prefab;

        [Button]
        public void Construct()
        {
#if UNITY_EDITOR
            Clear();
            var size = GetComponent<RectTransform>().rect.size;

            var source = GetSource();
            var extractor = new AreasExtractor(source);

            var options = new AreaExtractOptions(
                size,
                _colorEpsilon,
                _simplifyIterations,
                _distanceThreshold,
                _erosionPixels);

            var areaDatas = extractor.Extract(options);
            var spawnedAreas = new List<Area>();

            foreach (var extracted in areaDatas)
            {
                var area = (PrefabUtility.InstantiatePrefab(_prefab, transform) as Area)!;
                area.transform.localPosition = Vector2.zero;
                spawnedAreas.Add(area);

                var datas = new List<AreaData>();

                foreach (var contour in extracted.Contours)
                {
                    var center = AreaDataExtensions.GetCenter(contour);

                    var data = new AreaData(contour.ToArray(), center);
                    datas.Add(data);
                }

                area.Construct(datas);
            }

            var orderedAreas = spawnedAreas.OrderByDescending(t => t.Position.y).ToList();

            for (var i = 0; i < orderedAreas.Count; i++)
            {
                var area = orderedAreas[i];
                area.name = $"Area_{i}";
                area.transform.SetSiblingIndex(i);
            }

            var colors = new List<LevelColorData>()
            {
                new(Color.white, Vector2.zero),
                new(Color.black, new Vector2(1080, 1080)),
                new(Color.red, new Vector2(0, 1080)),
                new(Color.blue, new Vector2(1080, 0))
            };
            
            foreach (var area in orderedAreas)
            {
                var center = area.Position;
                var color = AreaDataExtensions.GetInterpolatedColor(center, size.x, colors);
                area.Renderer.SetColor(color);
            }

            _level.Construct(orderedAreas.ToArray());

            foreach (var area in orderedAreas)
            {
                EditorUtility.SetDirty(area);
                EditorUtility.SetDirty(area.Renderer);
            }

            EditorUtility.SetDirty(_level);
            
            Texture2D GetSource()
            {
                var stage = PrefabStageUtility.GetPrefabStage(gameObject);
                var folderPath = Path.GetDirectoryName(stage.assetPath);

                var textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

                foreach (var textureGuid in textureGuids)
                {
                    var texturePath = AssetDatabase.GUIDToAssetPath(textureGuid);
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                    if (texture.name == name)
                        return texture;
                }

                throw new Exception($"Texture with name {name} not found in folder {folderPath}");
            }
#endif
        }

        private void Clear()
        {
            var areas = GetComponentsInChildren<Area>(true);
            var colors = GetComponentsInChildren<AreaColorGroup>(true);

            foreach (var area in areas)
                DestroyImmediate(area.gameObject);

            foreach (var color in colors)
                DestroyImmediate(color.gameObject);
        }
    }
}