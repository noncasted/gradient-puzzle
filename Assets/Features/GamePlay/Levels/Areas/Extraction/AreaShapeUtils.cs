using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.GamePlay
{
    public static class AreaShapeUtils
    {
        private const int ValidationBreak = 10;

        private static readonly Vector2Int[] OutlineDirections =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0),
        };

        private static readonly Vector2Int[] ContourDirections =
        {
            new(1, 0),
            new(1, 1),

            new(0, 1),
            new(-1, 1),
            new(-1, 0),

            new(-1, -1),
            new(0, -1),
            new(1, -1),
        };

        public static Vector2 GetCenter(List<Vector2> points)
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

        public static List<Vector2> NormalizePointsToRect(List<Vector2> points, Vector2 rectSize, Vector2 textureSize)
        {
            var normalizedPoints = new List<Vector2>();

            foreach (var point in points)
            {
                var normalized = point / textureSize * rectSize;
                normalized -= rectSize / 2f;

                normalizedPoints.Add(normalized);
            }

            return normalizedPoints;
        }

        public static List<Vector2> SimplifyContour(List<Vector2> points, float minDistance)
        {
            for (var i = 0; i < points.Count - 1; i++)
            {
                var current = points[i];
                var next = points[i + 1];

                var distance = Vector2.Distance(current, next);

                if (distance < minDistance)
                {
                    points.RemoveAt(i + 1);
                    i--;
                }
            }

            return points;
        }

        public static List<Vector2> ExtractContour(
            IReadOnlyList<Color32> pixels,
            Vector2Int size,
            int erosionPixels,
            AreaExtractedColor color)
        {
            var rawArea = ConvertToBinaryImage(pixels, size, color);
            rawArea.RemoveEndPoints();
            AddErosion(rawArea, erosionPixels);

            var outline = GetOutlinePoints(rawArea);
            outline = ValidatePoints(rawArea, outline);

            var contour = new List<Vector2Int>() { outline.First() };
            var passed = new HashSet<Vector2Int>() { outline.First() };

            for (var i = 0; i < outline.Count - 1; i++)
            {
                var point = contour.Last();

                var neighbour = GetNeighbour(point);

                if (neighbour == outline.First())
                    break;

                contour.Add(neighbour);
                passed.Add(neighbour);
            }

            return contour.Select(t => new Vector2(t.x, t.y)).ToList();

            Vector2Int GetNeighbour(Vector2Int point)
            {
                foreach (var direction in ContourDirections)
                {
                    var checkPoint = point + direction;

                    if (outline.Contains(checkPoint) == false)
                        continue;

                    if (passed.Contains(checkPoint) == true)
                        continue;

                    return checkPoint;
                }

                throw new Exception();
            }
        }

        private static RawArea ConvertToBinaryImage(
            IReadOnlyList<Color32> pixels,
            Vector2Int size,
            AreaExtractedColor targetColor)
        {
            var binaryImage = new bool[size.x, size.y];

            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var color = pixels[y * size.x + x];
                    binaryImage[y, x] = targetColor.IsEqual(color);
                }
            }

            var area = new RawArea(binaryImage);

            return area;
        }

        private static void RemoveEndPoints(this RawArea area)
        {
            area.Iterate(point =>
            {
                var hasNeighbour = false;

                point.IterateNeighbours(ContourDirections, checkPoint =>
                {
                    if (area.IsInside(checkPoint) == false)
                        return true;

                    if (area.GetValue(checkPoint) == false)
                        return true;

                    hasNeighbour = true;
                    return false;
                });

                if (hasNeighbour == false)
                    area.Clear(point);
            });
        }

        private static void AddErosion(RawArea area, int erosionPixels)
        {
            for (var i = 0; i < erosionPixels; i++)
            {
                var add = new List<Vector2Int>();

                area.Iterate(point =>
                {
                    point.IterateNeighbours(OutlineDirections, checkPoint =>
                    {
                        if (area.IsInside(checkPoint) == false)
                            return;

                        if (area.GetValue(checkPoint) == true)
                            return;

                        add.Add(checkPoint);
                    });
                });

                foreach (var point in add)
                    area.Set(point);
            }
        }

        private static List<Vector2Int> GetOutlinePoints(RawArea area)
        {
            var points = new List<Vector2Int>();

            area.Iterate(point =>
            {
                var isContour = false;

                point.IterateNeighbours(OutlineDirections, checkPoint =>
                {
                    if (area.IsInside(checkPoint) == false)
                    {
                        isContour = true;
                        return false;
                    }

                    if (area.GetValue(checkPoint) == true)
                        return true;

                    isContour = true;
                    return false;
                });

                if (isContour == true)
                    points.Add(point);
            });

            return points;
        }

        private static List<Vector2Int> ValidatePoints(this RawArea area, List<Vector2Int> points)
        {
            var i = 0;

            while (ProcessValidation() == false)
            {
                i++;

                if (i > ValidationBreak)
                    throw new Exception();
            }

            return points;

            bool ProcessValidation()
            {
                var count = 0;

                var toRemove = new List<Vector2Int>();

                foreach (var point in points)
                {
                    var neighbourCount = 0;

                    foreach (var direction in ContourDirections)
                    {
                        var checkPoint = point + direction;

                        if (area.IsInside(checkPoint) == false)
                            continue;

                        if (area.GetValue(checkPoint) == false)
                            continue;

                        neighbourCount++;
                    }

                    if (neighbourCount == 1)
                    {
                        area.Clear(point);
                        toRemove.Add(point);
                        count++;
                    }
                }

                foreach (var remove in toRemove)
                {
                    Debug.Log($"Remove in validation: {remove}");
                    points.Remove(remove);
                }

                return count == 0;
            }
        }
    }
}