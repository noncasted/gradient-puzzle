using Internal;

namespace Features
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