using Internal;

namespace Common.StateMachines
{
    public interface IStateHandle
    {
        IReadOnlyLifetime Lifetime { get; }

        void Exit();
    }
}