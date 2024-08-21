namespace Features.Services
{
    public interface ILevelsStorage
    {
        ILevelConfiguration Get(int levelIndex);
    }
}