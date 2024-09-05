using Global.GameLoops;
using Internal;
using UnityEngine;

namespace Features
{
    public class GamePlayLoaderFactory : BaseGameLoopFactory
    {
        protected override void Create(IScopeBuilder builder)
        {
            Debug.Log("Register game play loader");
            builder.Register<GamePlayLoader>()
                .As<IScopeLoadedAsync>();
        }
    }
}