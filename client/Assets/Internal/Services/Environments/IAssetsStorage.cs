using System.Collections.Generic;

namespace Internal
{
    public interface IAssetsStorage
    {
        IReadOnlyDictionary<string, List<EnvAsset>> Assets { get; }
        IReadOnlyDictionary<PlatformType, OptionsRegistry> Options { get; }
    }
}