using Cysharp.Threading.Tasks;
using GamePlay.Loop;
using GamePlay.Setup;
using Internal;

namespace Tools.SceneTools
{
    public class GameMock : MockBase
    {
        public override async UniTaskVoid Process()
        {
            var globalResult = await BootstrapGlobal();

            var scopeLoaderFactory = globalResult.Get<IServiceScopeLoader>();
            var localResult = await scopeLoaderFactory.LoadGameMock(globalResult.Scope);

            var loop = localResult.Get<IGameLoop>();
            loop.Process(localResult.Lifetime).Forget();
        }
    }
}