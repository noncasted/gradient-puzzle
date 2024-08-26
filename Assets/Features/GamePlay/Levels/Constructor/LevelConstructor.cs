using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class LevelConstructor : MonoBehaviour
    {
        [SerializeField] private int _simplifyIterations = 1;
        [SerializeField] private float _distanceThreshold = 1f;
        [SerializeField] private int _erosionPixels;
        [SerializeField] private byte _colorEpsilon = 10;
        [SerializeField] private Level _level;
        [SerializeField] private Area _prefab;
        [SerializeField] private Sprite _source;
        [SerializeField] private RectTransform _colorsTransform;
        [SerializeField] private RectTransform _selfTransform;

        [Button]
        private void UpdateColor()
        {
            var colorPoints = GetComponentsInChildren<LevelConstructorColorPoint>(true);
            var colors = colorPoints.Select(t => new LevelColorData(t.Color, t.Position)).ToArray();

            foreach (var area in _level.AreasInternal)
            {
                var color = AreaDataExtensions.GetInterpolatedColor(area.Data.Center, _selfTransform.rect.x, colors);
                color.a = 1f;

                var data = new AreaData(area.Data.Points.ToArray(), area.Data.Center, color);
                area.Construct(data);
            }

#if UNITY_EDITOR
            foreach (var area in _level.AreasInternal)
            {
                UnityEditor.EditorUtility.SetDirty(area);
                UnityEditor.EditorUtility.SetDirty(area.Renderer);
            }

            UnityEditor.EditorUtility.SetDirty(_level);
#endif
        }

        [Button]
        public void Construct()
        {
            Clear();

            var colorPoints = GetComponentsInChildren<LevelConstructorColorPoint>(true);
            var colors = colorPoints.Select(t => new LevelColorData(t.Color, t.Position)).ToArray();

            var spawnedAreas = new List<Area>();

            var extractor = new AreasExtractor(_source.texture);

            var options = new AreaExtractOptions(
                _selfTransform.rect.size,
                _colorEpsilon,
                _simplifyIterations,
                _distanceThreshold,
                _erosionPixels,
                colors);

            var areaDatas = extractor.Extract(options);

            foreach (var data in areaDatas)
            {
                var area = Instantiate(_prefab, transform);
                area.transform.localPosition = Vector2.zero;
                spawnedAreas.Add(area);

                area.Renderer.SetPoints(data.Points);
                area.Construct(data);
            }

            var orderedAreas = spawnedAreas.OrderByDescending(t => t.Position.y).ToList();

            for (var i = 0; i < orderedAreas.Count; i++)
            {
                var area = orderedAreas[i];
                area.name = $"Area_{i}";
                area.transform.SetSiblingIndex(i);
            }

            _colorsTransform.SetAsLastSibling();
            _level.Construct(orderedAreas.ToArray());

#if UNITY_EDITOR
            foreach (var area in orderedAreas)
            {
                UnityEditor.EditorUtility.SetDirty(area);
                UnityEditor.EditorUtility.SetDirty(area.Renderer);
            }

            UnityEditor.EditorUtility.SetDirty(_level);
#endif
        }

        private void Clear()
        {
            var areas = GetComponentsInChildren<Area>(true);

            foreach (var area in areas)
                DestroyImmediate(area.gameObject);
        }
    }
}