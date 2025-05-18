using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class AreaShapeDataExtensions
    {
        public static bool IsInside(this AreaShapeData shapeData, Vector2 position)
        {
            return shapeData.SystemPoints.IsInside(position);
        }

        public static bool IsInside(this IReadOnlyList<Vector2> points, Vector2 position)
        {
            var count = points.Count;

            if (count < 3)
                return false;

            var inside = false;

            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                var a = points[i];
                var b = points[j];

                var intersect = (a.y > position.y) != (b.y > position.y) &&
                                (position.x < (b.x - a.x) * (position.y - a.y) / (b.y - a.y) + a.x);

                if (intersect)
                    inside = !inside;
            }

            return inside;
        }

        public static Color GetInterpolatedColor(
            Vector2 center,
            float size,
            IReadOnlyList<LevelColorData> points)
        {
            var accumulatedColor = Color.black;
            var totalWeight = 0f;

            foreach (var point in points)
            {
                var pointPosition = point.Position;
                var distance = Vector2.Distance(center, pointPosition) / size;
                var weight = 1f / distance;

                accumulatedColor += point.Color * weight;
                totalWeight += weight;
            }

            var result = accumulatedColor / totalWeight;
            return result;
        }

        public static Vector2 GetCenter(this IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count == 0)
                return Vector2.zero;

            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var point in points)
            {
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;
                if (point.y < minY) minY = point.y;
                if (point.y > maxY) maxY = point.y;
            }

            var centerX = minX + (maxX - minX) / 2f;
            var centerY = minY + (maxY - minY) / 2f;

            return new Vector2(centerX, centerY);
        }

        public static Vector2 GetCenterOfMass(this IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count == 0)
                return Vector2.zero;

            if (points[0] != points[^1])
            {
                var pointsCopy = new List<Vector2>(points);
                pointsCopy.Add(points[0]);
                points = pointsCopy;
            }

            float signedArea = 0;
            float cx = 0;
            float cy = 0;

            for (var i = 0; i < points.Count - 1; i++)
            {
                var x0 = points[i].x;
                var y0 = points[i].y;
                var x1 = points[i + 1].x;
                var y1 = points[i + 1].y;

                var a = x0 * y1 - x1 * y0;
                signedArea += a;
                cx += (x0 + x1) * a;
                cy += (y0 + y1) * a;
            }

            signedArea *= 0.5f;
            cx /= (6.0f * signedArea);
            cy /= (6.0f * signedArea);

            return new Vector2(cx, cy);
        }


        public static (Vector2, Vector2) GetBounds(this IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count == 0)
                return (Vector2.zero, Vector2.zero);

            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var point in points)
            {
                if (point.x < minX) minX = point.x;
                if (point.x > maxX) maxX = point.x;
                if (point.y < minY) minY = point.y;
                if (point.y > maxY) maxY = point.y;
            }

            return (new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        public static Vector2 GetSize(this IReadOnlyList<Vector2> points)
        {
            var (min, max) = points.GetBounds();

            var size = max - min;
            return size;
        }
    }
}