using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class TextureLevelExtractorExtensions
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

        public static IReadOnlyList<AreaExtractedColor> GetColors(this Texture2D texture, byte epsilon)
        {
            var pixels = texture.GetPixels32();
            var colors = new List<AreaExtractedColor>();

            foreach (var color in pixels)
            {
                if (color.a < 255)
                    continue;

                if (IsUnique(color) == false)
                    continue;

                var extractedColor = new AreaExtractedColor(color, epsilon);
                colors.Add(extractedColor);
            }

            return colors.ToList();

            bool IsUnique(Color32 color)
            {
                foreach (var check in colors)
                {
                    if (check.IsEqual(color))
                        return false;
                }

                return true;
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

        private static AreaPixels ConvertToBinaryImage(
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

            var area = new AreaPixels(binaryImage);

            return area;
        }

        private static void RemoveEndPoints(this AreaPixels areaPixels)
        {
            areaPixels.Iterate(point =>
            {
                var hasNeighbour = false;

                point.IterateNeighbours(ContourDirections, checkPoint =>
                {
                    if (areaPixels.IsInside(checkPoint) == false)
                        return true;

                    if (areaPixels.GetValue(checkPoint) == false)
                        return true;

                    hasNeighbour = true;
                    return false;
                });

                if (hasNeighbour == false)
                    areaPixels.Clear(point);
            });
        }

        private static void AddErosion(AreaPixels areaPixels, int erosionPixels)
        {
            for (var i = 0; i < erosionPixels; i++)
            {
                var add = new List<Vector2Int>();

                areaPixels.Iterate(point =>
                {
                    point.IterateNeighbours(OutlineDirections, checkPoint =>
                    {
                        if (areaPixels.IsInside(checkPoint) == false)
                            return;

                        if (areaPixels.GetValue(checkPoint) == true)
                            return;

                        add.Add(checkPoint);
                    });
                });

                foreach (var point in add)
                    areaPixels.Set(point);
            }
        }

        private static List<Vector2Int> GetOutlinePoints(AreaPixels areaPixels)
        {
            var points = new List<Vector2Int>();

            areaPixels.Iterate(point =>
            {
                var isContour = false;

                point.IterateNeighbours(OutlineDirections, checkPoint =>
                {
                    if (areaPixels.IsInside(checkPoint) == false)
                    {
                        isContour = true;
                        return false;
                    }

                    if (areaPixels.GetValue(checkPoint) == true)
                        return true;

                    isContour = true;
                    return false;
                });

                if (isContour == true)
                    points.Add(point);
            });

            return points;
        }

        private static List<Vector2Int> ValidatePoints(this AreaPixels areaPixels, List<Vector2Int> points)
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

                        if (areaPixels.IsInside(checkPoint) == false)
                            continue;

                        if (areaPixels.GetValue(checkPoint) == false)
                            continue;

                        neighbourCount++;
                    }

                    if (neighbourCount == 1)
                    {
                        areaPixels.Clear(point);
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