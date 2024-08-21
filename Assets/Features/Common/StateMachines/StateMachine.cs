using System;
using Features.Common.StateMachines.Abstract;
using Internal;

namespace Features.Common.StateMachines
{
    public class StateMachine : IStateMachine, IScopeSetup
    {
        private readonly ViewableDelegate _exited = new();
        private IReadOnlyLifetime _entityLifetime;

        private IState _currentState;
        private ILifetime _currentLifetime;

        public IViewableDelegate Exited => _exited;

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            _entityLifetime = lifetime;
        }

        public bool IsAvailable(IState state)
        {
            if (_currentState == null)
                return true;

            if (_currentState.Definition == state.Definition)
                return false;

            var result = _currentState.Definition.IsTransitable(state.Definition);

            return result;
        }

        public IStateHandle CreateHandle(IState state)
        {
            _currentLifetime?.Terminate();
            _currentState = state;

            _currentLifetime = _entityLifetime.Child();
            var handle = new StateHandle(state, this, _currentLifetime);

            return handle;
        }

        public void Exit(IState state)
        {
            if (state != _currentState)
                throw new Exception();

            _currentLifetime?.Terminate();
            _currentLifetime = null;
            _currentState = null;

            _exited?.Invoke();
        }
    }
}