namespace Features.Services
{
    public interface ILevelsStorage
    {
        int Count { get; }
        
        ILevelConfiguration Get(int levelIndex);
    }
}