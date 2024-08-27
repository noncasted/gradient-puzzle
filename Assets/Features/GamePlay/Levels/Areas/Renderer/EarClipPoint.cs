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

                var dirNext = EarClipPointExtensions.GetDirection(Point, Next.Point);
                var dirPrev = EarClipPointExtensions.GetDirection(Previous.Point, Point);
                var cwSign = _isClockwise ? 1 : -1;

                _reflex = cwSign * EarClipPointExtensions.Determinant(dirPrev, dirNext) >= -0.001f
                    ? ReflexState.Reflex
                    : ReflexState.Convex;

                return _reflex;
            }
        }
    }

    public static class EarClipPointExtensions
    {
        public static bool IsPointInside(this EarClipPoint source, EarClipPoint check, float aMargin, float bMargin)
        {
            return PointInsideTriangle(
                source.Next.Point,
                source.Point,
                source.Previous.Point,
                check.Point,
                0f,
                bMargin);
        }

        public static bool PointInsideTriangle(
            Vector2 a,
            Vector2 b,
            Vector2 c,
            Vector2 point,
            float aMargin = 0f,
            float bMargin = 0f,
            float cMargin = 0f)
        {
            var d0 = Determinant(GetDirection(a, b), GetDirection(a, point));
            var d1 = Determinant(GetDirection(b, c), GetDirection(b, point));
            var d2 = Determinant(GetDirection(c, a), GetDirection(c, point));
            var b0 = d0 < cMargin;
            var b1 = d1 < aMargin;
            var b2 = d2 < bMargin;
            return b0 == b1 && b1 == b2; // on the same side of all halfspaces, this can only happen inside
        }

        public static float Determinant(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

        public static Vector2 GetDirection(Vector2 a, Vector2 b)
        {
            var dx = b.x - a.x;
            var dy = b.y - a.y;
            var mag = Mathf.Sqrt(dx * dx + dy * dy);
            return new Vector2(dx / mag, dy / mag);
        }
    }
}