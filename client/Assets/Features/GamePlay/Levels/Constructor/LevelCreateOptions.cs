using System;
using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelCreateOptions
    {
        public LevelCreateOptions(
            Extraction extraction,
            Geometry geometry,
            Optimization optimization)
        {
            Extract = extraction;
            Geometries = geometry;
            Optimizations = optimization;
        }

        public Extraction Extract { get; }
        public Geometry Geometries { get; }
        public Optimization Optimizations { get; }

        public class Extraction
        {
            public Extraction(
                string svgPath,
                string inkscapePath,
                string inkscapeActions)
            {
                SvgPath = svgPath;
                InkscapePath = inkscapePath;
                InkscapeActions = inkscapeActions;
            }

            public string SvgPath { get; }
            public string InkscapePath { get; }
            public string InkscapeActions { get; }
        }

        [Serializable]
        public struct Geometry
        {
            public Geometry(
                Vector2 rectSize,
                Vector2 offset,
                float scale,
                float pointsDensity,
                float innerOffset,
                float minDistance)
            {
                _rectSize = rectSize;
                _offset = offset;
                _scale = scale;
                _pointsDensity = pointsDensity;
                _innerOffset = innerOffset;
                _minDistance = minDistance;
            }

            [SerializeField] private Vector2 _rectSize;
            [SerializeField] private Vector2 _offset;
            [SerializeField] private float _scale;
            [SerializeField] private float _pointsDensity;
            [SerializeField] private float _innerOffset;
            [SerializeField] private float _minDistance;

            public Vector2 RectSize => _rectSize;
            public Vector2 Offset => _offset;
            public float Scale => _scale;
            public float PointsDensity => _pointsDensity;
            public float InnerOffset => _innerOffset;
            public float MinDistance => _minDistance;
        }

        [Serializable]
        public struct Optimization
        {
            public Optimization(Simplify render, Simplify system)
            {
                _render = render;
                _system = system;
            }

            [SerializeField] private Simplify _render;
            [SerializeField] private Simplify _system;

            public Simplify Render => _render;
            public Simplify System => _system;
        }

        [Serializable]
        public struct Simplify
        {
            public Simplify(int iterations, float angle, float maxDistance)
            {
                _iterations = iterations;
                _angle = angle;
                _maxDistance = maxDistance;
            }

            [SerializeField] private int _iterations;
            [SerializeField] private float _maxDistance;
            [SerializeField] private float _angle;

            public int Iterations => _iterations;
            public float MaxDistance => _maxDistance;
            public float Angle => _angle;
        }
    }
}