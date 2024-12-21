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
    public class MobileGameInput : IGameInput, IScopeSetup, IUpdatable
    {
        public MobileGameInput(
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
        }

        public void OnUpdate(float delta)
        { 
            if (Input.touchCount != 1)
            {
                _cursorPosition = Vector2.zero;
                _worldPosition = Vector2.zero;
                _action.Set(false);
                
                return;
            }

            var touch = Input.GetTouch(0);
            var screenPosition = touch.position;

            _cursorPosition = _positionConverter.ScreenToLocal(screenPosition);
            _worldPosition = _positionConverter.ScreenToWorld(screenPosition);
            
            _action.Set(true);
        }

        public Vector2 GetInputInRect(RectTransform rect)
        {
            return _positionConverter.ScreenToLocal(Mouse.current.position.ReadValue());
        }
    }
}