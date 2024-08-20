using Global.Systems;
using Internal;

namespace Global.Inputs
{
    public class InputView : IInputView, IScopeSetup
    {
        public InputView(
            InputCallbacks callbacks,
            IUpdater updater,
            InputActions inputActions,
            Controls controls)
        {
            _callbacks = callbacks;
            _updater = updater;
            _inputActions = inputActions;
            _controls = controls;
        }

        private readonly InputCallbacks _callbacks;
        private readonly IUpdater _updater;
        private readonly InputActions _inputActions;
        private readonly Controls _controls;

        private readonly ViewableProperty<ILifetime> _lifetime = new();

        public Controls Controls => _controls;
        public IViewableProperty<ILifetime> ListenLifetime => _lifetime;

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            _lifetime.Set(new Lifetime());
            _controls.Enable();
            _callbacks.Invoke(_lifetime.Value);
            _updater.Add(lifetime, _inputActions);
        }
    }
}