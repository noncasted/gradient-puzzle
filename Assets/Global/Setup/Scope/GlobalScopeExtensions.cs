using Cysharp.Threading.Tasks;
using Global.Audio;
using Global.Backend;
using Global.Cameras;
using Global.GameLoops;
using Global.GameServices;
using Global.Inputs;
using Global.Publisher;
using Global.Systems;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer.Unity;

namespace Global.Setup
{
    public static class GlobalScopeExtensions
    {
        public static UniTask<IServiceScopeLoadResult> LoadGlobal(this IServiceScopeLoader loader, LifetimeScope parent)
        {
            Debug.Log($"Load global");
            var options = loader.Assets.GetAsset<GlobalScopeOptions>();
            return loader.Load(parent, options.Default, Construct);

            UniTask Construct(IScopeBuilder builder)
            {
                Debug.Log($"Construct globl services");

                builder
                    .AddAudio()
                    .AddCamera()
                    .AddInput()
                    .AddLoop()
                    .AddBackend()
                    .AddGameServices()
                    .AddSystemUtils();

                return UniTask.WhenAll(
                    builder.AddPublisher(),
                    builder.AddUI());
            }
        }

        public static UniTask<IServiceScopeLoadResult> LoadGlobalMock(
            this IServiceScopeLoader loader,
            LifetimeScope parent)
        {
            var options = loader.Assets.GetAsset<GlobalScopeOptions>();
            return loader.Load(parent, options.Mock, Construct);

            UniTask Construct(IScopeBuilder builder)
            {
                builder
                    .AddAudio()
                    .AddCamera()
                    .AddInput()
                    .AddBackend()
                    .AddGameServices()
                    .AddSystemUtils();

                return UniTask.WhenAll(
                    builder.AddPublisher(),
                    builder.AddUI());
            }
        }
    }
}