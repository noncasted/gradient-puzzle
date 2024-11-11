using System.Linq;
using GamePlay.Paints;
using Global.GameServices;
using Global.Inputs;
using Global.Systems;
using Internal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public class GameInput : IGameInput, IScopeSetup, IUpdatable
    {
        public GameInput(
            IUpdater updater,
            IPaintMoveArea moveArea,
            IGamePlayInputPositionConverter positionConverter,
            ILocalUserList users)
        {
            _updater = updater;
            _moveArea = moveArea;
            _positionConverter = positionConverter;
            _localUser = users.First().Input;
        }

        private readonly ViewableProperty<bool> _action = new(false);
        private readonly IUpdater _updater;
        private readonly IPaintMoveArea _moveArea;
        private readonly IGamePlayInputPositionConverter _positionConverter;
        private readonly IUserInput _localUser;

        private Vector2 _cursorPosition;
        private Vector2 _worldPosition;

        public IViewableProperty<bool> Action => _action;
        public Vector2 CursorPosition => _cursorPosition;
        public Vector2 WorldPosition => _worldPosition;

        public void OnSetup(IReadOnlyLifetime lifetime)
        {
            _updater.Add(lifetime, this);
            _localUser.Controls.GamePlay.Action.AttachFlag(lifetime, _action);
        }

        public void OnUpdate(float delta)
        {
            var screenPosition = Mouse.current.position.ReadValue();
            _cursorPosition = _positionConverter.ScreenToLocal(screenPosition);
            _worldPosition = _positionConverter.ScreenToWorld(screenPosition);
        }

        public Vector2 GetInputInRect(RectTransform rect)
        {
            return _positionConverter.ScreenToLocal(Mouse.current.position.ReadValue());
        }
    }
}