using System;
using System.Collections.Generic;

namespace Services
{
    public interface ILevelsStorage
    {
        IReadOnlyList<ILevelConfiguration> Configurations { get; }

        void OnLevelPassed(ILevelConfiguration configuration);
    }

    public static class LevelsStorageExtensions
    {
        public static ILevelConfiguration Get(this ILevelsStorage storage, int index)
        {
            return storage.Configurations[index];
        }

        public static int Count(this ILevelsStorage storage)
        {
            return storage.Configurations.Count;
        }

        public static ILevelConfiguration GetNext(this ILevelsStorage storage, ILevelConfiguration from)
        {
            var index = GetIndex();

            if (index == storage.Configurations.Count - 1)
                return storage.Configurations[0];

            return storage.Configurations[index + 1];

            int GetIndex()
            {
                for (var i = 0; i < storage.Configurations.Count; i++)
                {
                    var check = storage.Configurations[i];

                    if (check == from)
                        return i;
                }

                throw new Exception();
            }
        }
    }
}