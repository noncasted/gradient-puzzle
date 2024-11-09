using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaData
    {
        public AreaData(Vector2[] points, Vector2 center)
        {
            _points = points;
            _center = center;
        }

        [SerializeField] private Vector2[] _points;
        [SerializeField] private Vector2 _center;

        public IReadOnlyList<Vector2> Points => _points;
        public Vector2 Center => _center;
    }
}