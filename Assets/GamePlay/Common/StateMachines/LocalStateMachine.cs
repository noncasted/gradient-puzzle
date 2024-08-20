using System;
using Internal;

namespace Common.StateMachines
{
    public class LocalStateMachine : ILocalStateMachine, IScopeSetup
    {
        private readonly ViewableDelegate _exited = new();
        private IReadOnlyLifetime _entityLifetime;

        private ILocalState _currentLocalState;
        private ILifetime _currentLifetime;

        public IViewableDelegate Exited => _exited;

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            _entityLifetime = lifetime;
        }

        public bool IsAvailable(ILocalState localState)
        {
            if (_currentLocalState == null)
                return true;

            if (_currentLocalState.Definition == localState.Definition)
                return false;

            var result = _currentLocalState.Definition.IsTransitable(localState.Definition);

            return result;
        }

        public IStateHandle CreateHandle(ILocalState state, object payload = null)
        {
            _currentLifetime?.Terminate();
            _currentLocalState = state;

            _currentLifetime = _entityLifetime.Child();
            var handle = new StateHandle(state, this, _currentLifetime);

            return handle;
        }

        public void Exit(ILocalState localState)
        {
            if (localState != _currentLocalState)
                throw new Exception();

            _currentLifetime?.Terminate();
            _currentLifetime = null;
            _currentLocalState = null;

            _exited?.Invoke();
        }
    }
}