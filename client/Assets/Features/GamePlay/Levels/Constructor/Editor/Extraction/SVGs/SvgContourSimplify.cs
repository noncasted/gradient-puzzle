using System.Collections.Generic;
using Internal;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class SvgContourSimplify
    {
        public static void Simplify(this List<Vector2> points, float angleThreshold)
        {
            var previous = AngleBetween(points[0], points[1]);

            for (var i = 2; i < points.Count; i++)
            {
                var angle = AngleBetween(points[i - 1], points[i]);

                if (Mathf.Abs(angle - previous) < angleThreshold)
                {
                    points.RemoveAt(i - 1);
                    i--;
                }
                else
                {
                    previous = angle;
                }
            }

            float AngleBetween(Vector2 from, Vector2 to)
            {
                var direction = (to - from).normalized;
                return direction.ToAngle();
            }
        }
    }
}