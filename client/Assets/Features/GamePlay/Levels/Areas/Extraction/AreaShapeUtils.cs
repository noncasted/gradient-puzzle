using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamePlay.Levels
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

        public static void NormalizePointsToRect(List<Vector2> points, Vector2 rectSize, Vector2 textureSize)
        {
            for (var index = 0; index < points.Count; index++)
            {
                var point = points[index];
                var normalized = point / textureSize * rectSize;
                normalized -= rectSize / 2f;
                points[index] = normalized;
            }
        }

        public static void SimplifyContour(List<Vector2> points, float minDistance)
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
        }

        public static List<List<Vector2>> ExtractContours(
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

            var contours = new List<List<Vector2>>();

            while (outline.Count > 3)
            {
                var contour = GetContour();

                if (contour.Count < 3)
                    continue;

                contours.Add(contour);
            }

            return contours;

            List<Vector2> GetContour()
            {
                var contour = new List<Vector2Int>() { outline.First() };
                var passed = new HashSet<Vector2Int>() { outline.First() };

                var first = contour.First();

                while (true)
                {
                    var point = contour.Last();

                    var neighbour = GetNeighbour(point);

                    if (neighbour == first)
                        break;

                    contour.Add(neighbour);
                    passed.Add(neighbour);
                    outline.Remove(neighbour);
                    
                    continue;

                    Vector2Int GetNeighbour(Vector2Int from)
                    {
                        var passedFirst = false;
                        
                        foreach (var direction in ContourDirections)
                        {
                            var checkPoint = from + direction;

                            if (checkPoint == first)
                            {
                                passedFirst = true;
                                continue;
                            }
                            
                            if (outline.Contains(checkPoint) == false)
                                continue;

                            if (passed.Contains(checkPoint) == true)
                                continue;

                            return checkPoint;
                        }

                        if (passedFirst == true)
                            return first;
                        
                        throw new Exception();
                    }
                }

                outline.Remove(first);

                return contour.Select(t => new Vector2(t.x, t.y)).ToList();
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