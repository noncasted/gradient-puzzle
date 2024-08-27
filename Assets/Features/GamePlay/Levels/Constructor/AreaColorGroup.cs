using System;
using System.Collections.Generic;
using System.Linq;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class AreaColorGroup : MonoBehaviour
    {
        [SerializeField] private AreaColorPoint _pointPrefab;
        [SerializeField] private ColorPointsDictionary _points;

        private List<Area> _areas = new();
        private RectTransform _level;

        public IReadOnlyDictionary<AreaColorPoint, Color> Points => _points;
        public IReadOnlyList<Area> Areas => _areas;

        private void Awake()
        {
            DisablePoints();
        }

        public void EnablePoints()
        {
            foreach (var (point, _) in _points)
                point.gameObject.SetActive(true);
        }

        public void DisablePoints()
        {
            foreach (var (point, _) in _points)
                point.gameObject.SetActive(false);
        }
        
        [Button("Add point")]
        private void AddPoint()
        {
#if UNITY_EDITOR
            var point = UnityEditor.PrefabUtility.InstantiatePrefab(_pointPrefab, transform) as AreaColorPoint;
            point.transform.localPosition = RandomExtensions.RandomPosition(-300, 300);
            _points.Add(point, Color.white);
#endif
        }

        [Button("Reset")]
        public void ResetPoints()
        {
            var points = GetComponentsInChildren<AreaColorPoint>(true);
            
            foreach (var point in points)
                DestroyImmediate(point.gameObject);

            _points.Clear();

            AddPoint();
            AddPoint();
        }

        private bool Prepare()
        {
            if (_level == null)
            {
                var level = GetComponentInParent<Level>();

                if (level == null)
                    return false;
                
                _level = level.GetComponent<RectTransform>();
            }

            if (_areas == null || _areas.Count == 0)
                _areas = GetComponentsInChildren<Area>(true).ToList();

            return true;
        }

        public void UpdateColors()
        { 
            if (Prepare() == false)
                return;

            var size = _level.sizeDelta.x;
            var colors = new List<LevelColorData>();

            foreach (var (point, color) in _points)
            {
                var data = new LevelColorData(color, point.Position);
                colors.Add(data);
            }

            foreach (var area in _areas)
            {
                var center = area.Position;
                var color = AreaDataExtensions.GetInterpolatedColor(center, size, colors);
                area.Renderer.SetColor(color);
            }
        }
    }

    [Serializable]
    public class ColorPointsDictionary : SerializableDictionary<AreaColorPoint, Color>
    {
    }
}