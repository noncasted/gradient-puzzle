using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Features.GamePlay
{
    public static class AreaShapeUtils
    {
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

        public static bool IsBetween(this float x, float a, float b)
        {
            return (x - a) * (x - b) < 0;
        }


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
            var binaryImage = ConvertToBinaryImage(pixels, size, color);
            AddErosion(binaryImage, erosionPixels, size);

            var outline = GetOutlinePoints(binaryImage, size);
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

        private static bool[,] ConvertToBinaryImage(
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

            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    var point = new Vector2Int(x, y);
                    var hasNeighbour = false;

                    foreach (var direction in ContourDirections)
                    {
                        var checkPoint = point + direction;

                        if (checkPoint.x < 0 || checkPoint.x >= size.x || checkPoint.y < 0 || checkPoint.y >= size.y)
                            continue;

                        if (binaryImage[checkPoint.y, checkPoint.x] == false)
                            continue;

                        hasNeighbour = true;
                        break;
                    }

                    if (hasNeighbour == false && binaryImage[point.y, point.x] == true)
                        binaryImage[point.y, point.x] = false;
                }
            }

            return binaryImage;
        }

        private static void AddErosion(bool[,] source, int erosionPixels, Vector2 size)
        {
            for (var i = 0; i < erosionPixels; i++)
            {
                var add = new List<Vector2Int>();

                for (var y = 0; y < size.y; y++)
                {
                    for (var x = 0; x < size.x; x++)
                    {
                        if (source[y, x] == false)
                            continue;

                        var point = new Vector2Int(x, y);

                        foreach (var direction in OutlineDirections)
                        {
                            var checkPoint = point + direction;

                            if (checkPoint.x < 0 || checkPoint.x >= size.x || checkPoint.y < 0 ||
                                checkPoint.y >= size.y)
                                continue;

                            add.Add(checkPoint);
                        }
                    }
                }

                foreach (var point in add)
                    source[point.y, point.x] = true;
            }
        }

        private static List<Vector2Int> GetOutlinePoints(bool[,] source, Vector2 size)
        {
            var points = new List<Vector2Int>();

            for (var y = 0; y < size.y; y++)
            {
                for (var x = 0; x < size.x; x++)
                {
                    if (source[y, x] == false)
                        continue;

                    var point = new Vector2Int(x, y);

                    if (IsContour(point) == true)
                        points.Add(point);
                }
            }

            var i = 0;

            while (ValidatePoints() == false)
            {
                i++;

                if (i > 10)
                    throw new Exception();
            }

            return points;

            bool ValidatePoints()
            {
                var count = 0;

                var toRemove = new List<Vector2Int>();

                foreach (var point in points)
                {
                    var neighbourCount = 0;

                    foreach (var direction in ContourDirections)
                    {
                        var checkPoint = point + direction;

                        if (checkPoint.x < 0 || checkPoint.x >= size.x || checkPoint.y < 0 || checkPoint.y >= size.y)
                            continue;

                        if (source[checkPoint.y, checkPoint.x] == false)
                            continue;

                        neighbourCount++;
                    }

                    if (neighbourCount == 1)
                    {
                        source[point.y, point.x] = false;
                        toRemove.Add(point);
                        count++;
                    }
                }

                foreach (var remove in toRemove)
                {
                    Debug.Log($"Remove in valiadtion: {remove}");
                    points.Remove(remove);
                }

                return count == 0;
            }

            bool IsContour(Vector2Int point)
            {
                foreach (var direction in OutlineDirections)
                {
                    var checkPoint = point + direction;

                    if (checkPoint.x < 0 || checkPoint.x >= size.x || checkPoint.y < 0 || checkPoint.y >= size.y)
                        return true;

                    if (source[checkPoint.y, checkPoint.x] == false)
                        return true;
                }

                return false;
            }
        }
    }
}