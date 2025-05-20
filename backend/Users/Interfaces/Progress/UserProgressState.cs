namespace Users;

[GenerateSerializer]
public class UserProgressState
{
    [Id(0)] public Dictionary<int, int> PassedLevels { get; } = new();

    public void OnPassed(int stage, int level)
    {
        if (PassedLevels.TryGetValue(stage, out var currentLevel) && currentLevel > level)
            return;
        
        PassedLevels[stage] = level;
    }
}