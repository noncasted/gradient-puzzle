using Shapes;
using UnityEngine;

namespace Features.GamePlay
{
    public class EarClipPoint
    {
        private ReflexState _reflex = ReflexState.Unknown;

        public readonly int VertexIndex;
        public readonly Vector2 Point;
        private readonly bool _isClockwise;

        public EarClipPoint Previous;
        public EarClipPoint Next;

        public EarClipPoint(int vertexIndex, Vector2 point, bool isClockwise)
        {
            VertexIndex = vertexIndex;
            Point = point;
            _isClockwise = isClockwise;
        }

        public void MarkReflexUnknown() => _reflex = ReflexState.Unknown;

        public ReflexState ReflexState
        {
            get
            {
                if (_reflex != ReflexState.Unknown)
                    return _reflex;
                
                var dirNext = GetDirection(Point, Next.Point);
                var dirPrev = GetDirection(Previous.Point, Point);
                var cwSign = _isClockwise ? 1 : -1;
                
                _reflex = cwSign * ShapesMath.Determinant(dirPrev, dirNext) >= -0.001f
                    ? ReflexState.Reflex
                    : ReflexState.Convex;

                return _reflex;
            }
        }

        public static Vector2 GetDirection(Vector2 a, Vector2 b)
        {
            var dx = b.x - a.x;
            var dy = b.y - a.y;
            var mag = Mathf.Sqrt(dx * dx + dy * dy);
            return new Vector2(dx / mag, dy / mag);
        }

    }

    public static class EarClipPointExtensions
    {
        
        public static bool IsPointInside(this EarClipPoint source, EarClipPoint check, float aMargin, float bMargin)
        {
            return ShapesMath.PointInsideTriangle(
                source.Next.Point,
                source.Point,
                source.Previous.Point,
                check.Point,
                0f,
                bMargin);
        }
    }
}