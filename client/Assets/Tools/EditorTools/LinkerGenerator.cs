using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Tools
{
    public class LinkerGenerator : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            Generate();
        }

        [MenuItem("Tools/Generate link.xml")]
        public static void Generate()
        {
            var assetsDir = Application.dataPath;

            var linkXmlFilePath = Path.Combine(assetsDir, Application.dataPath + "/Settings/", "link.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(linkXmlFilePath) ??
                                      throw new InvalidOperationException(
                                          $"No directory in file name {linkXmlFilePath}"));

            var assembliesToPreserve = Enumerable.Empty<string>()
                .Concat(GetDllAssemblyNames(assetsDir))
                .Distinct()
                .OrderBy(s => s);

            var content = Enumerable.Empty<string>()
                .Concat("<linker>")
                .Concat(string.Empty)
                .Concat(assembliesToPreserve.Select(assemblyName =>
                    $"    <assembly fullname=\"{assemblyName}\" preserve=\"all\" />"))
                .Concat(string.Empty)
                .Concat("</linker>")
                .Aggregate(new StringBuilder(), (builder, line) => builder.AppendLine(line));

            using var fileStream = File.Open(linkXmlFilePath, FileMode.Create);
            using var streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(content);
        }

        private static IEnumerable<string> GetDllAssemblyNames(string assetsDir)
        {
            var allAssemblies = Directory.EnumerateFiles(assetsDir, "*.asmdef", SearchOption.AllDirectories)
                .Distinct()
                .ToList();

            var toRemove = allAssemblies.Where(t => t.Contains("Editor") || t.Contains("Tests") || t.Contains("Test"))
                .ToList();

            foreach (var removed in toRemove)
                allAssemblies.Remove(removed);

            return allAssemblies.Select(Path.GetFileNameWithoutExtension);
        }
    }

    public static class LinkerGeneratorExtensions
    {
        public static IEnumerable<TItem> Concat<TItem>(this IEnumerable<TItem> enumerable, TItem item) =>
            enumerable.Concat(new[] { item });
    }
}