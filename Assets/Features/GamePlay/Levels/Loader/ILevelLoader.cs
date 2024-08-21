using Features.Services;

namespace Features.GamePlay
{
    public interface ILevelLoader
    {
        ILevel Load(ILevelConfiguration configuration);
    }
}