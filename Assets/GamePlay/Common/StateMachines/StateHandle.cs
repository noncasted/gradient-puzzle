using Internal;

namespace Common.StateMachines
{
    public class StateHandle : IStateHandle
    {
        public StateHandle(
            ILocalState localState,
            ILocalStateMachine localStateMachine,
            IReadOnlyLifetime lifetime)
        {
            _localState = localState;
            _localStateMachine = localStateMachine;
            Lifetime = lifetime;
        }

        private readonly ILocalState _localState;
        private readonly ILocalStateMachine _localStateMachine;

        public IReadOnlyLifetime Lifetime { get; }

        public void Exit()
        {
            if (Lifetime.IsTerminated == true)
                return;

            _localStateMachine.Exit(_localState);
        }
    }
}