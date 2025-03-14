﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    public class AreaPixels
    {
        public AreaPixels(bool[,] value)
        {
            Value = value;
            Size = new Vector2Int(value.GetLength(1), value.GetLength(0));
        }

        public readonly bool[,] Value;
        public readonly Vector2Int Size;

        public void Set(Vector2Int point)
        {
            Value[point.y, point.x] = true;
        }

        public void Clear(Vector2Int point)
        {
            Value[point.y, point.x] = false;
        }
    }

    public static class AreaPixelsExtensions
    {
        public static void Iterate(this AreaPixels areaPixels, Action<Vector2Int> action)
        {
            var value = areaPixels.Value;
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

        public static bool IsInside(this AreaPixels areaPixels, Vector2Int point)
        {
            if (point.x < 0 || point.x >= areaPixels.Size.x || point.y < 0 || point.y >= areaPixels.Size.y)
                return false;

            return true;
        }

        public static bool GetValue(this AreaPixels areaPixels, Vector2Int point)
        {
            if (areaPixels.IsInside(point) == false)
                return false;

            if (areaPixels.Value[point.y, point.x] == false)
                return false;

            return true;
        }
    }
}