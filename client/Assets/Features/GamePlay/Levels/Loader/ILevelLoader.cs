using Services;

namespace GamePlay.Levels
{
    public interface ILevelLoader
    {
        ILevel Load(ILevelConfiguration configuration);
    }
}