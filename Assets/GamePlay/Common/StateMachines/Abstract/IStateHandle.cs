using Internal;

namespace GamePlay.Common
{
    public interface IStateHandle
    {
        IReadOnlyLifetime Lifetime { get; }

        void Exit();
    }
}