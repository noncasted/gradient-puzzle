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
                float centerRadiusStep,
                float centerDistanceStep, 
                float centerBorderDistance,
                float innerOffset)
            {
                _rectSize = rectSize;
                _offset = offset;
                _scale = scale;
                _pointsDensity = pointsDensity;
                _centerRadiusStep = centerRadiusStep;
                _centerDistanceStep = centerDistanceStep;
                _centerBorderDistance = centerBorderDistance;
                _innerOffset = innerOffset;
            }

            [SerializeField] private Vector2 _rectSize;
            [SerializeField] private Vector2 _offset;
            [SerializeField] private float _scale;
            [SerializeField] private float _pointsDensity;
            [SerializeField] private float _centerRadiusStep;
            [SerializeField] private float _centerDistanceStep;
            [SerializeField] private float _centerBorderDistance;
            [SerializeField] private float _innerOffset;

            public Vector2 RectSize => _rectSize;
            public Vector2 Offset => _offset;
            public float Scale => _scale;
            public float PointsDensity => _pointsDensity;
            public float CenterRadiusStep => _centerRadiusStep;
            public float CenterDistanceStep => _centerDistanceStep;
            public float CenterBorderDistance => _centerBorderDistance;
            public float InnerOffset => _innerOffset;
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