namespace Users;

public interface IUserProgress : IGrainWithGuidKey
{
    Task<UserProgressState> GetProgress();
    Task OnLevelPassed(int stage, int level);
}