using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GamePlay.Levels
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
                        convertedPoint *= _options.SvgScale;

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

                    for (var i = 0; i < path.ResultPoints.Count; i++)
                        path.ResultPoints[i] += _options.Offset;
                }
            }

            foreach (var area in rawAreas)
            {
                foreach (var path in area.Paths)
                {
                    for (var i = 0; i < path.ResultPoints.Count; i++)
                    {
                        var halfSize = _options.RectSize / 2f;
                        var point = path.ResultPoints[i];

                        if (point.x < -halfSize.x)
                            point.x = -halfSize.x;

                        if (point.y < -halfSize.y)
                            point.y = -halfSize.y;

                        if (point.x > halfSize.x)
                            point.x = halfSize.x;

                        if (point.y > halfSize.y)
                            point.y = halfSize.y;

                        if (i != 0)
                        {
                            var distanceToPrevious = Vector2.Distance(path.ResultPoints[i - 1], point);

                            if (distanceToPrevious < 1)
                            {
                                path.ResultPoints.RemoveAt(i);
                                i--;
                            }
                        }

                        path.ResultPoints[i] = point;

                        // if (point.x < -halfSize.x ||
                        //     point.y < -halfSize.y ||
                        //     point.x > halfSize.x ||
                        //     point.y > halfSize.y)
                        // {
                        //     path.ResultPoints.RemoveAt(i);
                        //     i--;
                        // }
                    }
                }
            }

            foreach (var area in rawAreas)
            {
                for (var i = 0; i < area.Paths.Count; i++)
                {
                    var path = area.Paths[i];

                    if (path.ResultPoints.Count < 4)
                    {
                        area.Paths.RemoveAt(i);
                        i--;
                    }
                }
            }


            for (var i = 0; i < rawAreas.Count; i++)
            {
                var area = rawAreas[i];

                if (area.Paths.Count == 0)
                {
                    rawAreas.RemoveAt(i);
                    i--;
                }
            }

            foreach (var area in rawAreas)
            {
                foreach (var path in area.Paths)
                    path.ResultPoints.Simplify(_options.SimplifyAngle);
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

        private List<RawAreaData> GetRawAreas(string svgFilePath)
        {
            var svgDocument = XDocument.Load(svgFilePath);
            var paths = new List<(Color, RawPathData)>();
            var index = 0;

            foreach (var pathElement in svgDocument.Descendants("{http://www.w3.org/2000/svg}path"))
            {
                if (pathElement.Attribute("width") != null && pathElement.Attribute("width").Value == "1200")
                    continue;

                var d = pathElement.Attribute("d")!.Value;
                var id = pathElement.Attribute("id")!.Value;
                var color = ExtractColor();

                var data = new RawPathData(
                    d,
                    index,
                    color,
                    id);

                paths.Add((color, data));

                Color ExtractColor()
                {
                    var fill = pathElement.Attribute("style")?.Value;

                    if (string.IsNullOrEmpty(fill) || !fill.StartsWith("fill:#"))
                        fill = pathElement.Attribute("fill")?.Value.Replace("#", "fill:#");

                    var hexColor = fill!.Substring(6);

                    if (hexColor.Length != 6)
                    {
                        Debug.LogWarning("Hex color code should be 6 characters long.");
                        return Color.clear;
                    }

                    var r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    var g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                    var b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                    return new Color(r / 255f, g / 255f, b / 255f, 1f);
                }
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
        }

        private void ConvertToPaths()
        {
            var inputFilePath = _options.SvgPath;
            var rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            var inkscapePath = rootPath + _options.InkscapePath;

            var content = File.ReadAllText(inputFilePath);
            content = content.Replace("<rect width=\"1200\" height=\"1200\" fill=\"black\"/>", "");
            content = content.Replace(
                "<rect width=\"1200\" height=\"1200\" transform=\"translate(0 0.5)\" fill=\"black\"/>", "");

            File.WriteAllText(inputFilePath, content);

            var arguments =
                $"{inkscapePath} --export-plain-svg --actions={_options.InkscapeActions} --export-overwrite {inputFilePath}";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = inkscapePath,
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

                Debug.Log($"Inkscape output: {output}");
                Debug.Log($"Inkscape error: {error}");
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