using Common.Setup;
using Cysharp.Threading.Tasks;
using GamePlay.Levels;
using GamePlay.Selections;
using Global.Setup;
using Global.UI;
using Internal;
using Loop;
using Services;
using UnityEngine;
using VContainer;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintTestMock : MonoBehaviour
    {
        [SerializeField] private LevelConfiguration _level;
            
        private ILoadedScope _internalScope;

        private void Awake()
        {
            Bootstrap().Forget();
        }

        private async UniTask Bootstrap()
        {
            var internalConfig = AssetsExtensions.Environment.GetAsset<InternalScopeConfig>();
            var internalScopeLoader = new InternalScopeLoader(internalConfig);
            _internalScope = internalScopeLoader.Load();
            var scopeLoader = _internalScope.Container.Container.Resolve<IServiceScopeLoader>();

            var globalScope = await scopeLoader.LoadGlobal(_internalScope);
            globalScope.Container.Container.Resolve<ILoadingScreen>().HideGameLoading();

            var options = new ScopeLoadOptions(
                globalScope,
                scopeLoader.Assets.GetAsset<GamePlayServicesScene>(),
                Construct,
                false);

            var scope = await scopeLoader.Load(options);
            await scope.Initialize();

            var test = scope.Get<PaintTest>();
            await test.Run(scope.Lifetime);

            UniTask Construct(IScopeBuilder builder)
            {
                FindFirstObjectByType<SceneServicesFactory>().Create(builder);

                builder.Register<PaintTest>()
                    .WithParameter(_level);

                builder
                    .AddGamePlayLoop()
                    .AddGamePlayServices()
                    .AddLevels()
                    .AddPaintServices()
                    .AddSelection();

                return UniTask.CompletedTask;
            }
        }

        private void OnApplicationQuit()
        {
            _internalScope?.Dispose().Forget();
        }
    }
}