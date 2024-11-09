using GamePlay.Paints;
using Internal;

namespace GamePlay.Common
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