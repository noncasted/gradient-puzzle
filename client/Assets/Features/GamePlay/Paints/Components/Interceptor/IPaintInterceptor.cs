using GamePlay.Common;

namespace GamePlay.Paints
{
    public interface IPaintInterceptor
    {
        IPaintTarget Current { get; }
        
        void LockCurrent();
        void UnlockCurrent();
        void Attach(IPaintTarget target);
        void Detach();
    }
}