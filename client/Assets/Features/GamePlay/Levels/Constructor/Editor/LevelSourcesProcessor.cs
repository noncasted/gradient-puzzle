using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Internal;
using Services;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GamePlay.Levels
{
    public class LevelSourcesProcessor : OdinEditorWindow
    {
        private const string PrefabPath = "Assets/Features/GamePlay/Levels/Constructor/Options/Level_Base.prefab";
        private const string SourcePath = "Assets/Features/GamePlay/Options/Levels/Source/";
        private const string DestinationPath = "Assets/Features/GamePlay/Options/Levels/";

        [SerializeField] private LevelSectionType _sectionType = LevelSectionType.Beginner;

        [MenuItem("Tools/MyTool")]
        private static void OpenWindow()
        {
            GetWindow<LevelSourcesProcessor>().Show();
        }

        [Button]
        private void ExtractSources()
        {
            var sourcePath = $"{SourcePath}{_sectionType.ToName()}";
            var destinationPath = $"{DestinationPath}{_sectionType.ToName()}";
            var sectionName = _sectionType.ToName();

            if (!AssetDatabase.IsValidFolder(sourcePath))
            {
                Debug.LogError("Source path does not exist: " + sourcePath);
                return;
            }

            var sourceFiles = Directory.GetFiles(sourcePath, "*.svg")
                .Where(path => Path.GetFileName(path)[0] != '.')
                .OrderBy(t => int.Parse(Path.GetFileName(t).Split(".")[0]))
                .ToList();

            for (var i = 0; i < sourceFiles.Count; i++)
                ProcessLevel(i);

            void ProcessLevel(int index)
            {
                var levelFolderName = $"Level_{index}";
                var destFolder = Path.Combine(destinationPath, levelFolderName);
                var prefix = $"Level_{sectionName}_{index}";

                if (!AssetDatabase.IsValidFolder(destFolder))
                    AssetDatabase.CreateFolder(destinationPath, levelFolderName);

                var sourceFilePath = sourceFiles[index];

                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                var fileExtension = Path.GetExtension(sourceFilePath);
                var destFilePath = Path.Combine(destFolder, $"{prefix}_Image{fileExtension}").Replace("\\", "/");

                File.Copy(sourceFilePath, destFilePath, overwrite: true);

                var sourceCopyPath = Path.Combine(destFolder, $"{prefix}_Image_Source{fileExtension}")
                    .Replace("\\", "/");

                File.Copy(sourceFilePath, sourceCopyPath, overwrite: true);

                AssetDatabase.ImportAsset(destFilePath);
                AssetDatabase.ImportAsset(sourceCopyPath);

                var levelOptions = CreateInstance<LevelOptions>();
                var fileName = $"{prefix}_Options.asset";
                var levelOptionsPath = Path.Combine(destFolder, fileName).Replace("\\", "/");
                AssetDatabase.CreateAsset(levelOptions, levelOptionsPath);

                var levelPrefab = AssetDatabase.LoadAssetAtPath<Level>(PrefabPath);
                var prefabInstance = PrefabUtility.InstantiatePrefab(levelPrefab) as Level;

                var prefabPath = Path.Combine(destFolder, $"{prefix}_View.prefab")
                    .Replace("\\", "/");

                PrefabUtility.SaveAsPrefabAssetAndConnect(
                    prefabInstance.gameObject,
                    prefabPath,
                    InteractionMode.AutomatedAction);

                DestroyImmediate(prefabInstance.gameObject);

                var prefabAsset = AssetDatabase.LoadAssetAtPath<Level>(prefabPath);
                var imageAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(destFilePath);
                var sprite = GeneratePreview(destFilePath, prefix);

                levelOptions.Setup(prefabAsset, _sectionType, imageAsset, sprite);

                EditorUtility.SetDirty(levelOptions);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Sprite GeneratePreview(string imagePath, string levelPrefix)
            {
                var sourceOptions = AssetsExtensions.FindAsset<LevelConstructorOptions>();
                var rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
                var inkscapePath = rootPath + sourceOptions.InkscapePath;

                ExecuteInkscape($"--export-type=\"png\" {imagePath}");

                var previewPath = imagePath.Replace("svg", "png");
                AssetDatabase.ImportAsset(previewPath);

                var importer = (TextureImporter)AssetImporter.GetAtPath(previewPath);
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.maxTextureSize = 128;

                importer.SaveAndReimport();

                AssetDatabase.ImportAsset(previewPath, ImportAssetOptions.ForceUpdate);
                AssetDatabase.RenameAsset(previewPath, $"{levelPrefix}_Preview");

                var newPath = previewPath.Replace("Image", "Preview");
                var preview = AssetDatabase.LoadAssetAtPath<Sprite>(newPath);

                return preview;

                void ExecuteInkscape(string arguments)
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = inkscapePath,
                            Arguments = $"{arguments} {arguments}",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                    };

                    process.Start();
                    process.WaitForExit();
                }
            }
        }

        [Button]
        private void ConstructLevels()
        {
            var options = AssetsExtensions.FindAsset<LevelConstructorOptions>();
            var destinationPath = $"{DestinationPath}{_sectionType.ToName()}";

            var prefabGuids = AssetDatabase.FindAssets("t:LevelOptions", new[] { destinationPath });

            var levels = prefabGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<LevelOptions>)
                .Where(level => level is not null)
                .ToList();

            foreach (var levelOptions in levels)
                ConstructLevel(levelOptions, options);
        }

        private void ConstructLevel(LevelOptions levelOptions, LevelConstructorOptions constructorOptions)
        {
            Debug.Log("Constructing level: " + levelOptions.name);

            var stage = PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(levelOptions.Prefab));
            var spawnedAreas = new List<Area>();
            var level = stage.prefabContentsRoot.GetComponent<Level>();

            var extractedAreas = Extract();

            var existingAreas = level.GetComponentsInChildren<Area>(true);

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
                    area = (PrefabUtility.InstantiatePrefab(constructorOptions.AreaPrefab, level.transform) as Area)!;
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

            level.Construct(orderedAreas.ToArray());

            foreach (var area in orderedAreas)
            {
                EditorUtility.SetDirty(area);
                EditorUtility.SetDirty(area.Renderer);
            }

            EditorUtility.SetDirty(level);

            IReadOnlyList<ExtractedArea> Extract()
            {
                var source = levelOptions.ImagePath;
                var sourceOptions = AssetsExtensions.FindAsset<LevelConstructorOptions>();

                var options = new LevelCreateOptions(
                    new LevelCreateOptions.Extraction(
                        source,
                        sourceOptions.InkscapePath,
                        sourceOptions.InkscapeActions,
                        level.name.Replace("View", "")),
                    constructorOptions.Geometry,
                    constructorOptions.Render);

                var extractor = new LevelExtractor(options);
                return extractor.Extract();
            }

            void Clear()
            {
                var areas = level.GetComponentsInChildren<Area>(true);
                var colors = level.GetComponentsInChildren<AreaColorGroup>(true);

                foreach (var area in areas)
                    DestroyImmediate(area.gameObject);

                foreach (var color in colors)
                    DestroyImmediate(color.gameObject);
            }
        }
    }
}