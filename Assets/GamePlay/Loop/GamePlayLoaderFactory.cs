using Global.GameLoops;
using Internal;

namespace GamePlay.Loop
{
    public class GamePlayLoaderFactory : BaseGameLoopFactory
    {
        protected override void Create(IScopeBuilder builder)
        {
            builder.Register<GamePlayLoader>()
                .As<IScopeLoadedAsync>();
        }
    }
}