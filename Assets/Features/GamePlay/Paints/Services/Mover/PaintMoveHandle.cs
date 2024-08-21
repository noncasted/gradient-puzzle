using Internal;

namespace Features.GamePlay
{
    public class PaintMoveHandle : IPaintMoveHandle
    {
        public PaintMoveHandle(ILifetime lifetime, IPaintTarget source)
        {
            _lifetime = lifetime;
            Source = source;
        }

        private readonly ILifetime _lifetime;

        public IReadOnlyLifetime Lifetime => _lifetime;
        public IPaintTarget Source { get; }

        public void Dispose()
        {
            _lifetime.Terminate();
        }
    }
}