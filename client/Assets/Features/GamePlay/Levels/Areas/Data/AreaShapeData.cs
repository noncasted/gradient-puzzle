using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaShapeData
    {
        public AreaShapeData(Vector2[] points, Vector2[] centers)
        {
            _points = points;
            _centers = centers;
        }

        [SerializeField] private Vector2[] _points;
        [SerializeField] private Vector2[] _centers;

        public IReadOnlyList<Vector2> Points => _points;
        public IReadOnlyList<Vector2> Centers => _centers;
    }
}