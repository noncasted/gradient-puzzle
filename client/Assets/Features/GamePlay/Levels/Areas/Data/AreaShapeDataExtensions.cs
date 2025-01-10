using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class AreaShapeDataExtensions
    {
        public static float GetMinDistanceToBorder(this AreaShapeData shapeData, Vector2 position)
        {
            var minDistance = float.MaxValue;

            foreach (var point in shapeData.Points)
            {
                var distance = Vector2.Distance(position, point);

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        public static bool IsInside(this AreaShapeData shapeData, Vector2 position)
        {
            if (shapeData.Points.Count < 3)
                return false;

            var isInPolygon = false;
            var lastVertex = shapeData.Points[^1];

            foreach (var vertex in shapeData.Points)
            {
                if (position.y.IsBetween(lastVertex.y, vertex.y))
                {
                    double t = (position.y - lastVertex.y) / (vertex.y - lastVertex.y);
                    var x = t * (vertex.x - lastVertex.x) + lastVertex.x;
                    if (x >= position.x) isInPolygon = !isInPolygon;
                }
                else
                {
                    if (Mathf.Approximately(position.y, lastVertex.y) && position.x < lastVertex.x &&
                        vertex.y > position.y)
                        isInPolygon = !isInPolygon;
                    if (Mathf.Approximately(position.y, vertex.y) && position.x < vertex.x &&
                        lastVertex.y > position.y)
                        isInPolygon = !isInPolygon;
                }

                lastVertex = vertex;
            }

            return isInPolygon;
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

        private static bool IsBetween(this float x, float a, float b)
        {
            return (x - a) * (x - b) < 0;
        }

        public static Vector2 GetCenter(IReadOnlyList<Vector2> points)
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

        public static Vector2 GetCenter(IReadOnlyList<AreaShapeData> datas)
        {
            var centers = new List<Vector2>();

            foreach (var data in datas)
                centers.Add(data.Center);

            return GetCenter(centers);
        }
    }
}