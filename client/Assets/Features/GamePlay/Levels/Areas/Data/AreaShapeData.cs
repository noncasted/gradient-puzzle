using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaShapeData
    {
        public AreaShapeData(Vector2[] renderPoints, Vector2[] systemPoints, Vector2[] innerPoint)
        {
            _renderPoints = renderPoints;
            _systemPoints = systemPoints;
            _innerPoint = innerPoint;
        }

        [SerializeField] private Vector2[] _renderPoints;
        [SerializeField] private Vector2[] _systemPoints;
        [SerializeField] private Vector2[] _innerPoint;

        public IReadOnlyList<Vector2> RenderPoints => _renderPoints;
        public IReadOnlyList<Vector2> SystemPoints => _systemPoints;
        public IReadOnlyList<Vector2> InnerPoint => _innerPoint;
    }
}