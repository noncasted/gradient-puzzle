using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Internal
{
    public abstract class ScopeConfigBase : EnvAsset, IServiceScopeConfig
    {
        [SerializeField] private ServiceScopeData _data;

        public ServiceScopeData GetData(IAssetEnvironment assets) => _data;

        public abstract UniTask Construct(IScopeBuilder builder);
    }
}