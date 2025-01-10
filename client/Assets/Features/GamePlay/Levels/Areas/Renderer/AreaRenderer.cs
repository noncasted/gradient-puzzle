using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class AreaRenderer : MonoBehaviour
    {
        [SerializeField] private AreaShapeRenderer _prefab;
        [SerializeField] private List<AreaShapeRenderer> _renderers;

        [SerializeField] private Color _color;

        public Color Color => _color;
        public IReadOnlyList<AreaShapeRenderer> Renderers => _renderers;

        public void Construct(IReadOnlyList<AreaShapeData> shapesData)
        {
            var renderers = GetComponentsInChildren<AreaShapeRenderer>(true);

            foreach (var areaRenderer in renderers)
                DestroyImmediate(areaRenderer.gameObject);

            _renderers.Clear();

            foreach (var data in shapesData)
            {
#if UNITY_EDITOR
                var areaRenderer = PrefabUtility.InstantiatePrefab(_prefab, transform) as AreaShapeRenderer;
                areaRenderer.SetPoints(data.Points);

                _renderers.Add(areaRenderer);
#endif
            }
        }

        public void SetMaterial(Material material)
        {
            foreach (var areaRenderer in _renderers)
            {
                areaRenderer.material = material;
            }
        }

        public void SetColor(Color color)
        {
            color.a = 1f;
            _color = color;

            foreach (var areaRenderer in _renderers)
                areaRenderer.color = color;
        }

        public void SetDirty()
        {
#if UNITY_EDITOR
            foreach (var areaRenderer in _renderers)
                EditorUtility.SetDirty(areaRenderer);
#endif
        }
    }
}