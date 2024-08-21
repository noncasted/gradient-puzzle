using System.Collections.Generic;

namespace Features.Services
{
    public class LevelsStorage : ILevelsStorage
    {
        public LevelsStorage(LevelsStorageOptions options)
        {
            _levels = options.Objects;
        }

        private readonly IReadOnlyList<LevelConfiguration> _levels;

        public ILevelConfiguration Get(int levelIndex)
        {
            return _levels[levelIndex];
        }
    }
}