using Internal;

namespace GamePlay.Loop
{
    public static class GameLoopExtensions
    {
        public static IScopeBuilder AddGamePlayLoop(this IScopeBuilder builder)
        {
            builder.Register<GameLoop>()
                .As<IGameLoop>();

            return builder;
        } 
    }
}