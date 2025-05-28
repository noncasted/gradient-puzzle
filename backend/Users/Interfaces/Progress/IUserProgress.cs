using Shared;

namespace Users;

public interface IUserProgress : IGrainWithGuidKey
{
    Task<UserProgressState> GetProgress();
    Task OnLevelPassed(LevelSectionType stage, int level);
}