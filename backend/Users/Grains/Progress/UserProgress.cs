namespace Users;

public class UserProgress : Grain, IUserProgress
{
    public UserProgress(
        [PersistentState(nameof(UserProgressState))]
        IPersistentState<UserProgressState> state)
    {
        _state = state;
    }

    private readonly IPersistentState<UserProgressState> _state;

    public Task<UserProgressState> GetProgress()
    {
        return Task.FromResult(_state.State);
    }

    public Task OnLevelPassed(string levelId)
    {
        _state.State.OnPassed(levelId);
        return _state.WriteStateAsync();
    }
}