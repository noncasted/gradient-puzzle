using Internal;

namespace Loop
{
    public static class GameLoopExtensions
    {
        public static IScopeBuilder AddGamePlayLoop(this IScopeBuilder builder)
        {
            builder.Register<GameLoop>()
                .WithAsset<GameLoopCheats>()
                .As<IGameLoop>();

            return builder;
        } 
    }
}