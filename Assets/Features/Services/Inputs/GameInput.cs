using Global.Inputs;
using Global.Systems;
using Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Services.Inputs
{
    public class GameInput : IGameInput, IScopeSetup, IUpdatable
    {
        public GameInput(
            IUpdater updater,
            IGamePlayInputPositionConverter positionConverter,
            Controls.GamePlayActions actions)
        {
            _updater = updater;
            _positionConverter = positionConverter;
            _actions = actions;
        }

        private readonly ViewableProperty<bool> _action = new(false);
        private readonly IUpdater _updater;
        private readonly IGamePlayInputPositionConverter _positionConverter;
        private readonly Controls.GamePlayActions _actions;

        private Vector2 _cursorPosition;

        public IViewableProperty<bool> Action => _action;
        public Vector2 CursorPosition => _cursorPosition;
        
        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            _updater.Add(lifetime, this);
            _actions.Action.AttachFlag(lifetime, _action);
        }

        public void OnUpdate(float delta)
        {
            _cursorPosition = _positionConverter.ScreenToLocal(Mouse.current.position.ReadValue());
        }
    }
}