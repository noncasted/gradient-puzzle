using Internal;

namespace Features.Common.StateMachines.Abstract
{
    public interface IStateHandle
    {
        IReadOnlyLifetime Lifetime { get; }

        void Exit();
    }
}