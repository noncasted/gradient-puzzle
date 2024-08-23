using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.GamePlay
{
    [Serializable]
    public class AreaData
    {
        public AreaData(Vector2[] points, Vector2 center, Color color)
        {
            _points = points;
            _center = center;
            _color = color;
        }

        [SerializeField] private Vector2[] _points;
        [SerializeField] private Vector2 _center;
        [SerializeField] private Color _color;

        public IReadOnlyList<Vector2> Points => _points;
        public Vector2 Center => _center;
        public Color Color => _color;
    }
}