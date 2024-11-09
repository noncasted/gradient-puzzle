using System;
using System.Collections.Generic;
using VContainer;

namespace Internal
{
    public class ScopeEventListeners : IScopeEventListeners
    {
        public ScopeEventListeners()
        {
            var events = new Dictionary<Type, object>();

            events.Add(typeof(IScopeSetup), new ScopeSetupResolver());
            events.Add(typeof(IScopeSetupCompletion), new ScopeCompletionSetupResolver());

            EventResolvers = events;
        }

        public IReadOnlyDictionary<Type, object> EventResolvers { get; }

        public void Add<T>(T listener) where T : class
        {
            var type = typeof(T);
            ((IEventResolver<T>)EventResolvers[type]).Add(listener);
        }

        public void Register(IContainerBuilder builder)
        {
            foreach (var (type, resolver) in EventResolvers)
            {
                builder.RegisterInstance(resolver)
                    .As(type);
            }
        }
    }
}