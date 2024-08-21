using Internal;

namespace Features.GamePlay
{
    public interface IPaintTarget
    {
        IViewableProperty<bool> IsTouched { get; }
    }
}