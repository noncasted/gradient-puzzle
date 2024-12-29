using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class AreaShapeUtils
    {
        public static void NormalizePointsToRect(List<Vector2> points, Vector2 rectSize, Vector2 sourceSize)
        {
            for (var index = 0; index < points.Count; index++)
            {
                var point = points[index];
                var normalized = point / sourceSize * rectSize;
                normalized -= rectSize / 2f;
                points[index] = normalized;
            }
        }

        public static void SimplifyContour(List<Vector2> points, float minDistance)
        {
            for (var i = 1; i < points.Count - 1; i++)
            {
                var previous = points[i - 1];
                var current = points[i];
                var next = points[i + 1];
                
                var angle = GetAngle(previous, current, next);
                
                if ((angle < 100 || angle > 250) && Mathf.Approximately(angle, 180f) == false)
                    continue;

                var distance = Vector2.Distance(previous, current);

                if (distance < minDistance)
                {
                    points.RemoveAt(i);
                    i--;
                }
            }
        }
        
        private static float GetAngle(Vector2 previous, Vector2 current, Vector2 next)
        {
            var vectorToPrev = (previous - current).normalized;
            var vectorToNext = (next - current).normalized;
            
            float dotProduct = Vector2.Dot(vectorToPrev, vectorToNext);
            float angleRadians = MathF.Acos(Math.Clamp(dotProduct, -1f, 1f));
            float angleDegrees = angleRadians * (180f / MathF.PI);
            
            return angleDegrees;
        }
    }
}