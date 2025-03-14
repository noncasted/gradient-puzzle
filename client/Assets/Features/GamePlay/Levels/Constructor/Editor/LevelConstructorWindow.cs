using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamePlay.Common;
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
        [SerializeField] [ShowIf(nameof(_isTexture))]
        private int _textureSimplifyIterations = 1;

        [SerializeField] [ShowIf(nameof(_isTexture))]
        private float _textureDistanceThreshold = 1f;

        [SerializeField] [ShowIf(nameof(_isTexture))]
        private int _textureErosionPixels;

        [SerializeField] [ShowIf(nameof(_isTexture))]
        private byte _textureColorEpsilon = 10;

        [SerializeField] [ShowIf(nameof(_isSvg))]
        private float _svgPointDensity = 0.1f;

        [SerializeField] [ShowIf(nameof(_isSvg))]
        private Vector2 _svgOffset;

        [SerializeField] [ShowIf(nameof(_isSvg))]
        private float _svgSimplifyAngle = 1f;

        [SerializeField] [ShowIf(nameof(_isSvg))]
        private float _svgScale = 0.9f;

        [SerializeField] private float _centerCheckDistance;
        [SerializeField] private AreaCenter _centerPrefab;
        [SerializeField] private Area _prefab;

        private bool _isTexture => LevelExtractionUtils.Type == LevelExtractionType.Texture;
        private bool _isSvg => LevelExtractionUtils.Type == LevelExtractionType.SVG;

        private Level _level;

        public void Setup(Level level)
        {
            _level = level;

            var textureOptions = AssetsExtensions.FindAsset<TextureLevelConstructorOptions>();
            var svgOptions = AssetsExtensions.FindAsset<SvgLevelConstructorOptions>();

            _textureSimplifyIterations = textureOptions.SimplifyIterations;
            _textureDistanceThreshold = textureOptions.DistanceThreshold;
            _textureErosionPixels = textureOptions.ErosionPixels;
            _textureColorEpsilon = textureOptions.ColorEpsilon;

            _svgPointDensity = svgOptions.PointsDensity;
            _svgSimplifyAngle = svgOptions.SimplifyAngle;
            _svgScale = svgOptions.Scale;

            _prefab = textureOptions.Prefab;
            _centerPrefab = svgOptions.CenterPrefab;
            _centerCheckDistance = svgOptions.CenterCheckDistance;
        }

        [Button("Construct Level")]
        public void Construct()
        {
            if (_level == null || _prefab == null)
            {
                Debug.LogError("Please assign a Level and Prefab reference.");
                return;
            }

            var spawnedAreas = new List<Area>();
            var extractedAreas = Extract();

            var existingAreas = _level.GetComponentsInChildren<Area>(true);

            if (extractedAreas.Count != existingAreas.Length)
                Clear();

            var centers = _level.GetComponentsInChildren<AreaCenter>(true);
            
            foreach (var center in centers)
                DestroyImmediate(center.gameObject);

            foreach (var extracted in extractedAreas)
            {
                var existing = existingAreas.FirstOrDefault(t => t.Id == extracted.Name);
                Area area;
                var color = extracted.Color;

                var shapesData = new List<AreaShapeData>();

                foreach (var contour in extracted.Contours)
                {
                    var data = new AreaShapeData(contour.Points.ToArray(), contour.Centers.ToArray());
                    shapesData.Add(data);
                }

                if (existing == null)
                {
                    area = (PrefabUtility.InstantiatePrefab(_prefab, _level.transform) as Area)!;
                    area.transform.localPosition = Vector2.zero;
                    area.name = extracted.Name;

                    area.Construct(shapesData, color, extracted.Order, extracted.Name);
                }
                else
                {
                    area = existing;
                    existing.UpdateShapes(shapesData);
                }

                spawnedAreas.Add(area);
            }

            foreach (var area in spawnedAreas)
            {
                foreach (var shape in area.Shapes)
                {
                    foreach (var center in shape.Centers)
                    {
                        var centerObject =
                            PrefabUtility.InstantiatePrefab(_centerPrefab, area.SelfTransform) as AreaCenter;

                        var size = 0f;

                        foreach (var point in shape.Points)
                        {
                            var distance = Vector2.Distance(point, center);
                            
                            if (distance > size)
                                size = distance;
                        }
                        
                        centerObject.Setup(center, size * 2.5f);
                    }
                }
            }

            var orderedAreas = spawnedAreas.OrderByDescending(t => t.Order).ToList();

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

        private IReadOnlyList<ExtractedArea> Extract()
        {
            var size = _level.GetComponent<RectTransform>().rect.size;

            switch (LevelExtractionUtils.Type)
            {
                case LevelExtractionType.Texture:
                {
                    var source = GetTextureSource();
                    var extractor = new TextureLevelExtractor(source);

                    var options = new TextureLevelExtractOptions(
                        size,
                        _textureColorEpsilon,
                        _textureSimplifyIterations,
                        _textureDistanceThreshold,
                        _textureErosionPixels);

                    return extractor.Extract(options);
                }
                case LevelExtractionType.SVG:
                {
                    var source = GetSvgSource();
                    var svgOptions = AssetsExtensions.FindAsset<SvgLevelConstructorOptions>();

                    var options = new SvgLevelExtractOptions(
                        source,
                        svgOptions.InkscapePath,
                        svgOptions.InkscapeActions,
                        size,
                        _svgOffset,
                        _svgPointDensity,
                        _svgSimplifyAngle,
                        _svgScale,
                        _centerCheckDistance);

                    var extractor = new SvgLevelExtractor(options);
                    return extractor.Extract();
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }


            Texture2D GetTextureSource()
            {
                var stage = PrefabStageUtility.GetPrefabStage(_level.gameObject);
                var folderPath = Path.GetDirectoryName(stage.assetPath);

                var textureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

                foreach (var textureGuid in textureGuids)
                {
                    var texturePath = AssetDatabase.GUIDToAssetPath(textureGuid);
                    var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

                    if (texture.name == _level.name)
                        return texture;
                }

                throw new Exception($"Texture with name {_level.name} not found in folder {folderPath}");
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
}