using Internal;

namespace Common.StateMachines
{
    public class StateHandle : IStateHandle
    {
        public StateHandle(
            IState state,
            IStateMachine stateMachine,
            IReadOnlyLifetime lifetime)
        {
            _state = state;
            _stateMachine = stateMachine;
            Lifetime = lifetime;
        }

        private readonly IState _state;
        private readonly IStateMachine _stateMachine;

        public IReadOnlyLifetime Lifetime { get; }

        public void Exit()
        {
            if (Lifetime.IsTerminated == true)
                return;

            _stateMachine.Exit(_state);
        }
    }
}