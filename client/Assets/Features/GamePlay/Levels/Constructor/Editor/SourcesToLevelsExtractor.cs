using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class SourcesToLevelsExtractor
    {
        [MenuItem("Tools/Convert sources to levels")]
        public static void ConvertSourcesToLevels()
        {
            var path = Application.dataPath + "/Features/GamePlay/Options/Levels/";

            var sources = Directory.GetFiles(path + "/Source/", "*.svg");

            var nameToPath = new Dictionary<string, string>();
            var names = new List<string>();

            foreach (var source in sources)
            {
                var name = ExtractName(source);
                nameToPath.Add(name, source);
                names.Add(name);
            }

            var orderedNames = names.OrderBy(t => t).ToList();

            for (var i = 0; i < orderedNames.Count; i++)
            {
                var sourcePath = nameToPath[orderedNames[i]];
                var targetPath = path + $"Level_{i}/Level_{i}.svg";
                var targetCopyPath = path + $"Level_{i}/Level_{i}_Source.svg";

                File.Copy(sourcePath, targetPath, true);
                File.Copy(sourcePath, targetCopyPath, true);
            }

            string ExtractName(string input)
            {
                var fileName = input.Split("/")[^1].Replace(".svg", "");

                var zerosCount = 5 - fileName.Length;

                return new string('0', zerosCount) + fileName;
            }
        }
    }
}