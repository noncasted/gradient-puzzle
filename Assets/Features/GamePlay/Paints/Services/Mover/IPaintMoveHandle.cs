using Internal;

namespace Features.GamePlay
{
    public interface IPaintMoveHandle
    {
        IReadOnlyLifetime Lifetime { get; }
        IPaintTarget Source { get; }
    }
}