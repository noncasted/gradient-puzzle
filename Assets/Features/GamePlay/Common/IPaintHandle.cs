using Internal;

namespace Features.GamePlay
{
    public interface IPaintHandle
    {
        IViewableProperty<IPaint> Paint { get; }
        bool IsLocked { get; }
        
        void Set(IPaint paint);
        void Clear(IPaint paint);
        void Lock();
        void Unlock();
    }
}