using System.Collections.Generic;
using System.IO;
using System.Linq;
using Internal;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelConstructorWindow : OdinEditorWindow
    {
        [SerializeField] private Area _areaPrefab;

        [SerializeField] private LevelCreateOptions.Geometry _geometry;
        [SerializeField] private LevelCreateOptions.Optimization _optimization;

        [SerializeField] private LevelConstructorOptions _source;
        
        private Level _level;

        public void Setup(Level level)
        {
            _level = level;

            var options = AssetsExtensions.FindAsset<LevelConstructorOptions>();
            _areaPrefab = options.AreaPrefab;
            _geometry = options.Geometry;
            _optimization = options.Render;
            _source = options;
        }

        [Button("Construct Level")]
        public void Construct()
        {
            if (_level == null || _areaPrefab == null)
            {
                Debug.LogError("Please assign a Level and Prefab reference.");
                return;
            }

            var spawnedAreas = new List<Area>();
            var extractedAreas = Extract();

            var existingAreas = _level.GetComponentsInChildren<Area>(true);

            if (extractedAreas.Count != existingAreas.Length)
                Clear();

            foreach (var extracted in extractedAreas)
            {
                var existing = existingAreas.FirstOrDefault(t => t.Id == extracted.Name);
                Area area;
                var color = extracted.Color;

                var shapesData = new List<AreaShapeData>();

                foreach (var contour in extracted.Contours)
                {
                    var data = new AreaShapeData(
                        contour.RenderPoints.ToArray(),
                        contour.SystemPoints.ToArray(),
                        contour.InnerPoints.ToArray());
                    
                    shapesData.Add(data);
                }

                if (existing == null)
                {
                    area = (PrefabUtility.InstantiatePrefab(_areaPrefab, _level.transform) as Area)!;
                    area.transform.localPosition = Vector2.zero;
                    area.name = extracted.Name;

                    area.Construct(shapesData, color, extracted.Order, extracted.Name);
                    area.RecalculateInnerPoints();
                }
                else
                {
                    area = existing;
                    existing.UpdateShapes(shapesData);
                    area.RecalculateInnerPoints();
                }

                spawnedAreas.Add(area);
            }
            
            var orderedAreas = spawnedAreas.OrderBy(t => t.Order).ToList();

            for (var i = 0; i < orderedAreas.Count; i++)
            {
                var area = orderedAreas[i];
                area.name = $"Area_{i}";
                area.transform.SetSiblingIndex(i);
            }

            _level.Construct(orderedAreas.ToArray());

            foreach (var area in orderedAreas)
            {
                EditorUtility.SetDirty(area);
                EditorUtility.SetDirty(area.Renderer);
            }

            EditorUtility.SetDirty(_level);
        }

        [Button("Clear")]
        private void Clear()
        {
            var areas = _level.GetComponentsInChildren<Area>(true);
            var colors = _level.GetComponentsInChildren<AreaColorGroup>(true);

            foreach (var area in areas)
                DestroyImmediate(area.gameObject);

            foreach (var color in colors)
                DestroyImmediate(color.gameObject);
        }

        [Button("SetOptionsOverride")]
        private void SetOptionsOverride()
        {
            _level.SetOptions(_geometry, _optimization);
            EditorUtility.SetDirty(_level);
        }

        [Button("LoadOptionsOverride")]
        private void LoadOptionsOverride()
        {
            _geometry = _level.Geometry;
            _optimization = _level.Optimization;
        }

        private IReadOnlyList<ExtractedArea> Extract()
        {
            var source = GetSvgSource();
            var sourceOptions = AssetsExtensions.FindAsset<LevelConstructorOptions>();

            var options = new LevelCreateOptions(
                new LevelCreateOptions.Extraction(source, sourceOptions.InkscapePath, sourceOptions.InkscapeActions),
                _geometry,
                _optimization);

            var extractor = new LevelExtractor(options);
            return extractor.Extract();
        }

        string GetSvgSource()
        {
            var stage = PrefabStageUtility.GetPrefabStage(_level.gameObject);
            var folderPath = Path.GetDirectoryName(stage.assetPath);
            var levelName = _level.name;
            return Path.GetFullPath(Path.Combine(folderPath, $"{levelName}.svg"));
        }
    }
}