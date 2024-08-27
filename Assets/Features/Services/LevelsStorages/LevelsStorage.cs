using System.Collections.Generic;

namespace Features.Services
{
    public class LevelsStorage : ILevelsStorage
    {
        public LevelsStorage(LevelsStorageOptions options)
        {
            Configurations = options.Objects;
        }

        public IReadOnlyList<ILevelConfiguration> Configurations { get; }
    }
}