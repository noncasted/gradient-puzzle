using Cysharp.Threading.Tasks;
using Features.GamePlay;
using Features.Services;
using Internal;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Features.Common.Setup
{
    public static class GamePlayScopeExtensions
    {
        public static async UniTask<IServiceScopeLoadResult> ProcessGamePlay(
            this IServiceScopeLoader loader,
            LifetimeScope parent)
        {
            Debug.Log("Process game play load");
            var options = loader.Assets.GetAsset<GamePlayScopeOptions>();
            var scopeLoadResult = await loader.Load(parent, options.Default, Construct);
            Debug.Log("Game scope loaded");
            await scopeLoadResult.EventLoop.RunLoaded(scopeLoadResult.Lifetime);

            Debug.Log("Game event loop executed");
            var loop = scopeLoadResult.Scope.Container.Resolve<IGameLoop>();
            await loop.Process(scopeLoadResult.Lifetime);
            return scopeLoadResult;

            UniTask Construct(IScopeBuilder builder)
            {
                Debug.Log("Construct game play services");

                builder
                    .AddGamePlayLoop()
                    .AddGamePlayServices()
                    .AddLevels()
                    .AddPaintServices()
                    .AddSelection();
                
                return builder.AddScene();
            }
        }

        public static async UniTask<IServiceScopeLoadResult> LoadGameMock(
            this IServiceScopeLoader loader,
            LifetimeScope parent)
        {
            var options = loader.Assets.GetAsset<GamePlayScopeOptions>();
            var scopeLoadResult = await loader.Load(parent, options.Default, Construct);
            await scopeLoadResult.EventLoop.RunLoaded(scopeLoadResult.Lifetime);

            return scopeLoadResult;

            UniTask Construct(IScopeBuilder builder)
            {
                Object.FindFirstObjectByType<SceneServicesFactory>().Create(builder);

                builder
                    .AddGamePlayLoop()
                    .AddGamePlayServices()
                    .AddLevels()
                    .AddPaintServices()
                    .AddSelection();

                return UniTask.CompletedTask;
            }
        }

        private static UniTask AddScene(this IScopeBuilder builder)
        {
            return builder.FindOrLoadSceneWithServices<GamePlayScene>();
        }
    }
}