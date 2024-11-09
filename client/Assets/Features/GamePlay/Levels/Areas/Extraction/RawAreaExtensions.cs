using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class RawAreaExtensions
    {
        public static void Iterate(this RawArea area, Action<Vector2Int> action)
        {
            var value = area.Value;
            var xSize = value.GetLength(0);
            var ySize = value.GetLength(1);

            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    if (value[y, x] == true)
                        action.Invoke(new Vector2Int(x, y));
                }
            }
        }

        public static void IterateNeighbours(
            this Vector2Int point,
            IReadOnlyList<Vector2Int> directions,
            Func<Vector2Int, bool> action)
        {
            foreach (var direction in directions)
            {
                var checkPoint = point + direction;

                var continueIteration = action.Invoke(checkPoint);

                if (continueIteration == false)
                    break;
            }
        }
        
        public static void IterateNeighbours(
            this Vector2Int point,
            IReadOnlyList<Vector2Int> directions,
            Action<Vector2Int> action)
        {
            point.IterateNeighbours(directions, checkPoint =>
            {
                action.Invoke(checkPoint);
                return true;
            });
        }

        public static bool IsInside(this RawArea area, Vector2Int point)
        {
            if (point.x < 0 || point.x >= area.Size.x || point.y < 0 || point.y >= area.Size.y)
                return false;

            return true;
        }

        public static bool GetValue(this RawArea area, Vector2Int point)
        {
            if (area.IsInside(point) == false)
                return false;

            if (area.Value[point.y, point.x] == false)
                return false;

            return true;
        }
    }
}