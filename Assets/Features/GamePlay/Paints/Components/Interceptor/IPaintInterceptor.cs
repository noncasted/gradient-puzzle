namespace Features.GamePlay
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