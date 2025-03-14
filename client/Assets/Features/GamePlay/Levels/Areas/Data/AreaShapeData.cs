using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaShapeData
    {
        public AreaShapeData(Vector2[] renderPoints, Vector2[] systemPoints, Vector2[] centers)
        {
            _renderPoints = renderPoints;
            _systemPoints = systemPoints;
            _centers = centers;
        }

        [SerializeField] private Vector2[] _renderPoints;
        [SerializeField] private Vector2[] _systemPoints;
        [SerializeField] private Vector2[] _centers;

        public IReadOnlyList<Vector2> RenderPoints => _renderPoints;
        public IReadOnlyList<Vector2> SystemPoints => _systemPoints;
        public IReadOnlyList<Vector2> Centers => _centers;
    }
}