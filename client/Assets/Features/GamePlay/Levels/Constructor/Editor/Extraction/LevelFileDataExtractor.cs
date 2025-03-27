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
    public class LevelFileDataExtractor
    {
        private readonly LevelCreateOptions _options;

        public LevelFileDataExtractor(LevelCreateOptions options)
        {
            _options = options;
        }
        
        public List<AreaData> GetRawAreas()
        {
            ConvertToPaths();
            
            var svgDocument = XDocument.Load(_options.Extract.SvgPath);
            var paths = new List<PathData>();
            var index = 0;

            foreach (var pathElement in svgDocument.Descendants("{http://www.w3.org/2000/svg}path"))
            {
                if (pathElement.Attribute("width") != null && pathElement.Attribute("width").Value == "1200")
                    continue;

                var d = pathElement.Attribute("d")!.Value;
                var id = pathElement.Attribute("id")!.Value;
                var color = ExtractColor();

                var data = new PathData(
                    d,
                    index,
                    color,
                    id);

                paths.Add(data);
                index++;
                
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

            var result = new Dictionary<Color, AreaData>();

            foreach (var path in paths)
            {
                var color = path.Color;

                if (result.TryGetValue(color, out var area) == false)
                {
                    area = new AreaData(new List<PathData>(), color);
                    result.Add(color, area);
                }

                area.Paths.Add(path);
            }

            foreach (var (_, data) in result)
            {
                var order = data.Paths.Min(t => t.Order);
                data.Order = order;
            }

            
            return result.Values.ToList();
        }

        private void ConvertToPaths()
        {
            var inputFilePath = _options.Extract.SvgPath;
            var rootPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            var inkscapePath = rootPath + _options.Extract.InkscapePath;

            var content = File.ReadAllText(inputFilePath);
            content = content.Replace("<rect width=\"1080\" height=\"1080\"/>", "");
            content = content.Replace("<rect width=\"1080\" height=\"1080\" fill=\"none\"/>", "");

            File.WriteAllText(inputFilePath, content);

            var arguments =
                $"{inkscapePath} --export-plain-svg --actions={_options.Extract.InkscapeActions} --export-overwrite {inputFilePath}";

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
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error executing Inkscape: {ex.Message}");
            }
        }

        public class PathData
        {
            public PathData(
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
        }

        public class AreaData
        {
            public AreaData(
                List<PathData> paths,
                Color color)
            {
                Paths = paths;
                Color = color;
            }

            public List<PathData> Paths { get; }
            public Color Color { get; }
            public int Order { get; set; }
        }
    }
}