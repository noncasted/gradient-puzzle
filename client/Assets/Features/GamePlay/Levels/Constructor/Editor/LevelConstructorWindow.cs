using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamePlay.Levels.SVGs;
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
        private float _svgPointDensity = 1f;
        
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
            
            _prefab = textureOptions.Prefab;
        }

        [Button("Construct Level")]
        public void Construct()
        {
            if (_level == null || _prefab == null)
            {
                Debug.LogError("Please assign a Level and Prefab reference.");
                return;
            }

            Clear();
            var size = _level.GetComponent<RectTransform>().rect.size;

            var spawnedAreas = new List<Area>();
            var extractedAreas = Extract();

            foreach (var extracted in extractedAreas)
            {
                var area = (PrefabUtility.InstantiatePrefab(_prefab, _level.transform) as Area)!;
                area.transform.localPosition = Vector2.zero;
                spawnedAreas.Add(area);

                var shapesData = new List<AreaShapeData>();

                foreach (var contour in extracted.Contours)
                {
                    var center = AreaShapeDataExtensions.GetCenter(contour);
                    var data = new AreaShapeData(contour.ToArray(), center);
                    
                    shapesData.Add(data);
                }

                area.Construct(shapesData, extracted.Color, extracted.Order);
            }

            var orderedAreas = spawnedAreas.OrderByDescending(t => t.Order).ToList();

            for (var i = 0; i < orderedAreas.Count; i++)
            {
                var area = orderedAreas[i];
                area.name = $"Area_{i}";
                area.transform.SetSiblingIndex(i);
            }

            // var colors = new List<LevelColorData>
            // {
            //     new(Color.white, Vector2.zero),
            //     new(Color.black, new Vector2(1080, 1080)),
            //     new(Color.red, new Vector2(0, 1080)),
            //     new(Color.blue, new Vector2(1080, 0))
            // };
            //
            // foreach (var area in orderedAreas)
            // {
            //     var center = area.Position;
            //     var color = AreaShapeDataExtensions.GetInterpolatedColor(center, size.x, colors);
            //     area.Renderer.SetColor(color);
            // }

            _level.Construct(orderedAreas.ToArray());

            foreach (var area in orderedAreas)
            {
                EditorUtility.SetDirty(area);
                EditorUtility.SetDirty(area.Renderer);
            }

            EditorUtility.SetDirty(_level);

            return;
        }

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
                        size, _svgPointDensity);
                    
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

                // Get the full system path by combining the folder path and the file name, and then getting the absolute path
                return Path.GetFullPath(Path.Combine(folderPath, $"{levelName}.svg"));
            }
        }
    }
}