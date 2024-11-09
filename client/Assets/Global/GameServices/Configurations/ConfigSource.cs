using Internal;

namespace Global.GameServices
{
    public abstract class ConfigSource : EnvAsset
    {
        public abstract void CreateInstance(IScopeBuilder builder);
    }
}