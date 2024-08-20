using Internal;

namespace Common.StateMachines
{
    public interface ILocalStateMachine
    {
        IViewableDelegate Exited { get; }

        bool IsAvailable(ILocalState localState);
        IStateHandle CreateHandle(ILocalState state, object payload = null);
        void Exit(ILocalState localState);
    }
}