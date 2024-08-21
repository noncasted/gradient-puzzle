using Internal;

namespace Features.GamePlay.Common
{
    public interface IPaintTarget
    {
        IViewableProperty<bool> IsTouched { get; }
    }
}