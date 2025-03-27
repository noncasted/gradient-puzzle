using System.Collections.Generic;
using Internal;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class ContourSimplify
    {
        public static void SimplifyForward(this List<Vector2> points, LevelCreateOptions.Simplify options)
        {
            for (var iteration = 0; iteration < options.Iterations; iteration++)
            {
                var previous = AngleBetween(points[0], points[1]);

                for (var i = 2; i < points.Count; i++)
                {
                    var angle = AngleBetween(points[i - 1], points[i]);
                    var distance = Vector2.Distance(points[i - 1], points[i]);

                    if (Mathf.Abs(angle - previous) < options.Angle && distance < options.MaxDistance)
                    {
                        points.RemoveAt(i - 1);
                        i--;
                    }
                    else
                    {
                        previous = angle;
                    }
                }
            }


            float AngleBetween(Vector2 from, Vector2 to)
            {
                var direction = (to - from).normalized;
                return direction.ToAngle();
            }
        }
        
        public static void SimplifyBackwards(this List<Vector2> points, LevelCreateOptions.Simplify options)
        {
            for (var iteration = 0; iteration < options.Iterations; iteration++)
            {
                var previous = AngleBetween(points[0], points[1]);

                for (var i = 2; i < points.Count; i++)
                {
                    var angle = AngleBetween(points[i - 1], points[i]);
                    var distance = Vector2.Distance(points[i - 1], points[i]);

                    if (Mathf.Abs(angle - previous) < options.Angle && distance < options.MaxDistance)
                    {
                        points.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        previous = angle;
                    }
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