using Internal;

namespace Common.StateMachines
{
    public interface IStateMachine
    {
        IViewableDelegate Exited { get; }

        bool IsAvailable(IState state);
        IStateHandle CreateHandle(IState state);
        void Exit(IState state);
    }
}