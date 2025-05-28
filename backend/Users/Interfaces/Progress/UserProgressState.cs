using Shared;

namespace Users;

[GenerateSerializer]
public class UserProgressState
{
    [Id(0)] public Dictionary<LevelSectionType, int> PassedLevels { get; } = new();

    public void OnPassed(LevelSectionType stage, int level)
    {
        if (PassedLevels.TryGetValue(stage, out var currentLevel) && currentLevel > level)
            return;
        
        PassedLevels[stage] = level;
    }
}