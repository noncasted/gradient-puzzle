using System.Collections.Generic;
using UnityEngine;

namespace Features.GamePlay
{
    public static class AreaDataExtensions
    {
        public static bool IsInside(this AreaData data, Vector2 position)
        {
            if (data.Points.Count < 3)
                return false;

            var isInPolygon = false;
            var lastVertex = data.Points[^1];

            foreach (var vertex in data.Points)
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
    }
}