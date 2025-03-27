using Global.GameLoops;
using Internal;
using UnityEngine;

namespace Loop
{
    public class GamePlayLoaderFactory : BaseGameLoopFactory
    {
        protected override void Create(IScopeBuilder builder)
        {
            builder.Register<GamePlayLoader>()
                .As<IGamePlayLoader>();
        }
    }
}