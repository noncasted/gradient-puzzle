using System;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Internal
{
    public interface IServiceScopeLoader
    {
        IAssetEnvironment Assets { get; }
        
        UniTask<IServiceScopeLoadResult> Load(
            LifetimeScope parent,
            ServiceScopeData data,
            Func<IScopeBuilder, UniTask> construct);

        UniTask<IServiceScopeLoadResult> Load(LifetimeScope parent, IServiceScopeConfig config);
    }
}