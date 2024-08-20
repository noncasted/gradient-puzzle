using Internal;

namespace Global.Inputs
{
    public interface IInputView
    {
        Controls Controls { get; }
        IViewableProperty<ILifetime> ListenLifetime { get; }
    }
}