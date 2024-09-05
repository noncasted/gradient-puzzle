using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class AreaCompoundRenderer : MonoBehaviour
    {
        [SerializeField] private AreaRenderer _prefab;
        [SerializeField] private List<AreaRenderer> _renderers;

        [SerializeField] private Color _color;

        public Color Color => _color;

        public void Construct(IReadOnlyList<AreaData> datas)
        {
            var renderers = GetComponentsInChildren<AreaRenderer>(true);

            foreach (var areaRenderer in renderers)
                DestroyImmediate(areaRenderer.gameObject);

            foreach (var data in datas)
            {
#if UNITY_EDITOR
                var areaRenderer = PrefabUtility.InstantiatePrefab(_prefab, transform) as AreaRenderer;
                areaRenderer.SetPoints(data.Points);

                _renderers.Add(areaRenderer);
#endif
            }
        }

        public void SetMaterial(Material material)
        {
            foreach (var areaRenderer in _renderers)
                areaRenderer.material = material;
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