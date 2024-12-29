using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GamePlay.Levels.SVGs
{
    public class SvgLevelExtractor
    {
        private readonly SvgLevelExtractOptions _options;

        public SvgLevelExtractor(SvgLevelExtractOptions options)
        {
            _options = options;
        }

        public IReadOnlyList<ExtractedArea> Extract()
        {
            ConvertToPaths();

            var rawAreas = GetRawAreas(_options.SvgPath);
            var areas = new List<ExtractedArea>();

            foreach (var area in rawAreas)
            {
                foreach (var path in area.Paths)
                {
                    var properties = new SvgPathProperties.SvgPath(path.D);
                    var length = properties.Length;
                    var points = new List<Vector2>();

                    for (var i = 0f; i < length; i += _options.PointsDensity)
                    {
                        var point = properties.GetPointAtLength(i);

                        var convertedPoint = new Vector2((float)point.X, (float)point.Y);
                        convertedPoint.y *= -1f;
                        points.Add(convertedPoint);
                    }

                    path.ResultPoints.AddRange(points);
                }
            }

            var centerOffset = GetCenterOffset();

            foreach (var area in rawAreas)
            {
                foreach (var path in area.Paths)
                {
                    for (var i = 0; i < path.ResultPoints.Count; i++)
                        path.ResultPoints[i] += centerOffset;
                }
            }

            foreach (var area in rawAreas)
            {
                var contours = new List<IReadOnlyList<Vector2>>();

                foreach (var path in area.Paths)
                    contours.Add(path.ResultPoints);

                var order = area.Paths.Min(t => t.Order);
                areas.Add(new ExtractedArea(contours, area.Color, order, area.Paths[0].Name));
            }

            return areas;

            Vector2 GetCenterOffset()
            {
                var maxX = float.NegativeInfinity;
                var maxY = float.NegativeInfinity;
                var minX = float.PositiveInfinity;
                var minY = float.PositiveInfinity;

                foreach (var area in rawAreas)
                {
                    foreach (var path in area.Paths)
                    {
                        foreach (var point in path.ResultPoints)
                        {
                            if (point.x > maxX)
                                maxX = point.x;

                            if (point.y > maxY)
                                maxY = point.y;

                            if (point.x < minX)
                                minX = point.x;

                            if (point.y < minY)
                                minY = point.y;
                        }
                    }
                }

                var result = new Vector2(minX + (maxX - minX) / 2f, minY + (maxY - minY) / 2f);
                result *= -1f;

                return result;
            }
        }

        private IReadOnlyList<RawAreaData> GetRawAreas(string svgFilePath)
        {
            var svgDocument = XDocument.Load(svgFilePath);
            var paths = new List<(Color, RawPathData)>();
            var index = 0;

            foreach (var pathElement in svgDocument.Descendants("{http://www.w3.org/2000/svg}path"))
            {
                var d = pathElement.Attribute("d")!.Value;
                var id = pathElement.Attribute("id")!.Value;
                var style = pathElement.Attribute("style")!.Value;

                var color = ExtractColor(style);

                var data = new RawPathData(
                    d,
                    index,
                    color,
                    id);

                paths.Add((color, data));
            }

            var result = new Dictionary<Color, RawAreaData>();

            foreach (var (color, path) in paths)
            {
                if (result.TryGetValue(color, out var area) == false)
                {
                    area = new RawAreaData(new List<RawPathData>(), color);
                    result.Add(color, area);
                }

                area.Paths.Add(path);
            }

            return result.Values.ToList();

            Color ExtractColor(string fill)
            {
                if (string.IsNullOrEmpty(fill) || !fill.StartsWith("fill:#"))
                {
                    Debug.LogWarning("Invalid fill attribute format.");
                    return Color.clear;
                }

                var hexColor = fill.Substring(6);

                if (hexColor.Length != 6)
                {
                    Debug.LogWarning("Hex color code should be 6 characters long.");
                    return Color.clear;
                }

                var r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                return new Color(r / 255f, g / 255f, b / 255f, 1f); // Alpha is fully opaque
            }
        }

        private void ConvertToPaths()
        {
            var inputFilePath = _options.SvgPath;
            var arguments = $"inkscape --export-plain-svg --actions={_options.InkscapeActions} --export-overwrite {inputFilePath}";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = _options.InkscapePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using var process = Process.Start(processStartInfo)!;
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing Inkscape: {ex.Message}");
            }
        }

        public class RawPathData
        {
            public RawPathData(
                string d,
                int order,
                Color color,
                string name)
            {
                D = d;
                Order = order;
                Color = color;
                Name = name;
            }

            public string D { get; }
            public int Order { get; }
            public Color Color { get; }
            public string Name { get; }

            public List<Vector2> ResultPoints { get; } = new();
        }

        public class RawAreaData
        {
            public RawAreaData(List<RawPathData> paths, Color color)
            {
                Paths = paths;
                Color = color;
            }

            public List<RawPathData> Paths { get; }
            public Color Color { get; }
        }
    }
}