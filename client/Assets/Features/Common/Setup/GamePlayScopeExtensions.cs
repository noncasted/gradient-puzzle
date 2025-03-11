using Cysharp.Threading.Tasks;
using GamePlay.Levels;
using GamePlay.Paints;
using GamePlay.Selections;
using Internal;
using Loop;
using Services;
using UnityEngine;
using VContainer;

namespace Common.Setup
{
    public static class GamePlayScopeExtensions
    {
        public static async UniTask<ILoadedScope> ProcessGamePlay(this IServiceScopeLoader loader, ILoadedScope parent)
        {
            var options = new ScopeLoadOptions(
                parent,
                loader.Assets.GetAsset<GamePlayServicesScene>(),
                Construct,
                false);
            
            var scope = await loader.Load(options);
            await scope.Initialize();

            var loop = scope.Container.Container.Resolve<IGameLoop>();
            await loop.Process(scope.Lifetime);
            return scope;

            UniTask Construct(IScopeBuilder builder)
            {
                builder
                    .AddGamePlayLoop()
                    .AddGamePlayServices()
                    .AddLevels()
                    .AddPaintServices()
                    .AddSelection();
                
                return builder.AddScene();
            }
        }

        public static async UniTask<ILoadedScope> LoadGameMock(this IServiceScopeLoader loader, ILoadedScope parent)
        {
            var options = new ScopeLoadOptions(
                parent,
                loader.Assets.GetAsset<GamePlayServicesScene>(),
                Construct,
                false);

            var scope = await loader.Load(options);
            await scope.Initialize();

            return scope;

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
            return builder.FindOrLoadSceneWithServices<GamePlayScene>(true);
        }
    }
}