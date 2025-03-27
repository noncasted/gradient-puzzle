using Common.Setup;
using Cysharp.Threading.Tasks;
using Internal;
using Loop;

namespace Tools
{
    public class GameMock : MockBase
    {
        public override async UniTaskVoid Process()
        {
            var globalResult = await BootstrapGlobal();

            var scopeLoaderFactory = globalResult.Get<IServiceScopeLoader>();
            var localResult = await scopeLoaderFactory.LoadGameMock(globalResult);

            var loop = localResult.Get<IGameLoop>();
            loop.Process(localResult.Lifetime).Forget();
        }
    }
}