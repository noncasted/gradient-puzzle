using Cysharp.Threading.Tasks;
using Features.Common.Setup;
using Features.Loop;
using Internal;

namespace Tools
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