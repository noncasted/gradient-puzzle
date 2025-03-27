namespace Users;

[GenerateSerializer]
public class UserProgressState
{
    [Id(0)] public HashSet<string> PassedLevels { get; } = new();

    public void OnPassed(string levelId)
    {
        PassedLevels.Add(levelId);
    }
}