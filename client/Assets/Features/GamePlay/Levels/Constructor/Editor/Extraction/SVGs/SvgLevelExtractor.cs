using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var paths = GetPathDAttributes(_options.SvgPath);
            var areas = new List<ExtractedArea>();
            var rawAreas = new List<List<List<Vector2>>>();

            foreach (var path in paths)
            {
                var properties = new SvgPathProperties.SvgPath(path.D);
                var length = properties.Length;
                var points = new List<Vector2>();

                for (var i = 0f; i < length; i += _options.PointsDensity)
                {
                    var point = properties.GetPointAtLength(i);

                    var convertedPoint = new Vector2((float)point.X, (float)point.Y);
                    var rotatedPoint = ApplyRotation(convertedPoint, path.Transform);
                    rotatedPoint.y *= -1f;
                    points.Add(rotatedPoint);
                }

                rawAreas.Add(new List<List<Vector2>>() { points });
            }

            var centerOffset = GetCenterOffset();

            foreach (var area in rawAreas)
            {
                foreach (var subArea in area)
                {
                    for (var i = 0; i < subArea.Count; i++)
                        subArea[i] += centerOffset;
                }
            }

            foreach (var area in rawAreas)
                areas.Add(new ExtractedArea(area));

            return areas;

            Vector2 ApplyRotation(Vector2 point, Vector3 transform)
            {
                var angleInRadians = Mathf.Deg2Rad * transform.x;

                var cosAngle = Mathf.Cos(angleInRadians);
                var sinAngle = Mathf.Sin(angleInRadians);

                var translatedX = point.x - transform.y;
                var translatedY = point.y - transform.z;
                var rotatedX = cosAngle * translatedX - sinAngle * translatedY;
                var rotatedY = sinAngle * translatedX + cosAngle * translatedY;

                return new Vector2(rotatedX + transform.y, rotatedY + transform.z);
            }

            Vector2 GetCenterOffset()
            {
                var maxX = float.NegativeInfinity;
                var maxY = float.NegativeInfinity;
                var minX = float.PositiveInfinity;
                var minY = float.PositiveInfinity;

                foreach (var area in rawAreas)
                {
                    foreach (var subArea in area)
                    {
                        foreach (var point in subArea)
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
                Debug.Log($"minX: {minX}, maxX: {maxX}, minY: {minY}, maxY: {maxY} offset: {result}");
                return result;
            }
        }

        private List<RawPathData> GetPathDAttributes(string svgFilePath)
        {
            var svgDocument = XDocument.Load(svgFilePath);
            var paths = new List<RawPathData>();
            var index = 0;

            foreach (var pathElement in svgDocument.Descendants("{http://www.w3.org/2000/svg}path"))
            {
                var d = pathElement.Attribute("d")!.Value;
                var id = pathElement.Attribute("id")!.Value;
                var style = pathElement.Attribute("style")!.Value;
                var rotation = ExtractRotation();

                paths.Add(new RawPathData(
                    d,
                    rotation,
                    index,
                    ExtractColor(style),
                    id));

                Vector3 ExtractRotation()
                {
                    var transform = pathElement.Attribute("transform")?.Value;

                    if (string.IsNullOrEmpty(transform))
                        return Vector3.zero;

                    transform = transform.Replace("rotate(", "").Replace(")", "");

                    var transformValues = transform.Split(' ');

                    return new Vector3(
                        float.Parse(transformValues[0]),
                        float.Parse(transformValues[1]),
                        float.Parse(transformValues[2]));
                }
            }

            return paths;
            
            Color ExtractColor(string fill)
            {
                if (string.IsNullOrEmpty(fill) || !fill.StartsWith("fill:#"))
                {
                    Debug.LogWarning("Invalid fill attribute format.");
                    return Color.clear; // Return transparent color if the format is invalid
                }

                // Remove "fill:#" and parse the hex string
                string hexColor = fill.Substring(6);

                if (hexColor.Length != 6)
                {
                    Debug.LogWarning("Hex color code should be 6 characters long.");
                    return Color.clear;
                }

                // Parse RGB components from the hex string
                byte r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                // Convert to Unity Color (values normalized between 0 and 1)
                return new Color(r / 255f, g / 255f, b / 255f, 1f); // Alpha is fully opaque
            }
        }

        private void ConvertToPaths()
        {
            var inputFilePath = _options.SvgPath;
            var arguments =
                $"inkscape --export-plain-svg --actions=\"select-all;object-to-path;;transform-remove\" --export-overwrite {inputFilePath}";

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
                Vector3 transform,
                int order,
                Color color,
                string name)
            {
                D = d;
                Transform = transform;
                Order = order;
                Color = color;
                Name = name;
            }

            public string D { get; }
            public Vector3 Transform { get; }
            public int Order { get; }
            public Color Color { get; }
            public string Name { get; }
        }
    }
}