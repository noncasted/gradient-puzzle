using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public interface ILevelsStorage
    {
        IReadOnlyDictionary<LevelSectionType, IReadOnlyList<ILevelData>> Sections { get; }

        void OnLevelPassed(ILevelData data);
        UniTask RecalculateUnlocks();
    }

    public static class LevelsStorageExtensions
    {
        public static ILevelData Get(this ILevelsStorage storage, LevelSectionType section, int index)
        {
            return storage.Sections[section][index];
        }

        public static int Count(this ILevelsStorage storage)
        {
            return storage.Sections.Count;
        }

        public static ILevelData GetNext(this ILevelsStorage storage, ILevelData from)
        {
            var section = storage.Sections[from.SectionType];
            var index = GetIndex();

            if (index == section.Count - 1)
                return section.First();
            
            var nextIndex = index + 1;
            return section[nextIndex];

            int GetIndex()
            {
                for (var i = 0; i < section.Count; i++)
                {
                    var check = section[i];

                    if (check == from)
                        return i;
                }

                throw new Exception();
            }
        }

        public static Dictionary<LevelSectionType, int> CalculateProgress(this ILevelsStorage storage)
        {
            var result = new Dictionary<LevelSectionType, int>();

            foreach (var section in storage.Sections)
            {
                var count = section.Value.Count;
                
                if (count == 0)
                {
                    result.Add(section.Key, 0);
                    continue;
                }   
                
                var passed = section.Value.Count(x => x.IsUnlocked.Value == true);

                result.Add(section.Key, (int)Math.Round((float)passed / count * 100));
            }

            return result;
        }
    }
}