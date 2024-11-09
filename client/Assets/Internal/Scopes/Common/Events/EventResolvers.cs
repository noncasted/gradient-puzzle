using System.Collections.Generic;
using UnityEngine;

namespace Internal
{
    public class ScopeSetupResolver : IScopeSetup, IEventResolver<IScopeSetup>
    {
        private readonly List<IScopeSetup> _listeners = new();

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            foreach (var listener in _listeners)
            {
                Debug.Log($"Invoke view event on {((MonoBehaviour)listener).name}: OnSetup to {listener.GetType().Name}");
                listener.OnSetup(lifetime);
            }
        }

        public void Add(IScopeSetup listener)
        {
            _listeners.Add(listener);
        }
    }

    public class ScopeCompletionSetupResolver : IScopeSetupCompletion, IEventResolver<IScopeSetupCompletion>
    {
        private readonly List<IScopeSetupCompletion> _listeners = new();

        public void OnSetupCompletion(IReadOnlyLifetime lifetime)
        {
            foreach (var listener in _listeners)
            {
                Debug.Log($"Invoke view event on {((MonoBehaviour)listener).name}: ScopeSetupCompletion to {listener.GetType().Name}");
                listener.OnSetupCompletion(lifetime);
            }
        }

        public void Add(IScopeSetupCompletion listener)
        {
            _listeners.Add(listener);
        }
    }
}