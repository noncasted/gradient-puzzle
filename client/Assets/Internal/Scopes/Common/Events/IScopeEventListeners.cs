using VContainer;

namespace Internal
{
    public interface IScopeEventListeners
    {
        void Add<T>(T listener) where T : class;
        void Register(IContainerBuilder builder);
    }

    public interface IEventResolver<T> where T : class
    {
        void Add(T listener);
    }

    public static class ScopeEventListenersExtensions
    {
        public static IScopeBuilder AddViewEvents<T>(this IScopeBuilder builder, T target) where T : class
        {
            if (target is IScopeBaseSetup baseSetup)
                builder.EventListeners.Add(baseSetup);

            if (target is IScopeBaseSetupAsync baseSetupAsync)
                builder.EventListeners.Add(baseSetupAsync);

            if (target is IScopeSetup setup)
                builder.EventListeners.Add(setup);

            if (target is IScopeSetupAsync setupAsync)
                builder.EventListeners.Add(setupAsync);

            if (target is IScopeSetupCompletion setupCompletion)
                builder.EventListeners.Add(setupCompletion);

            if (target is IScopeSetupCompletionAsync setupCompletionAsync)
                builder.EventListeners.Add(setupCompletionAsync);

            if (target is IScopeDispose dispose)
                builder.EventListeners.Add(dispose);

            if (target is IScopeDisposeAsync disposeAsync)
                builder.EventListeners.Add(disposeAsync);

            return builder;
        }
    }
}