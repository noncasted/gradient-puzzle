using System;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Selections;
using Global.Systems;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Paints
{
    public class PaintMover : IPaintMover
    {
        public PaintMover(
            IUpdater updater,
            IGameInput input,
            IPaintMoveArea moveArea,
            IPaintTransform transform,
            IPaintImage image,
            IPaintInterceptor interceptor,
            PaintMoverOptions options)
        {
            _updater = updater;
            _input = input;
            _moveArea = moveArea;
            _transform = transform;
            _image = image;
            _interceptor = interceptor;
            _options = options;
        }

        private readonly IUpdater _updater;
        private readonly IGameInput _input;
        private readonly IPaintMoveArea _moveArea;
        private readonly IPaintTransform _transform;
        private readonly IPaintImage _image;
        private readonly IPaintInterceptor _interceptor;
        private readonly PaintMoverOptions _options;

        public UniTask TransitTo(IReadOnlyLifetime lifetime, Transform target, IPaintTarget from)
        {
            var distance = Vector2.Distance(_transform.WorldPosition, target.position);
            var moveTime = _options.TransitTimeCurve.Evaluate(distance);
            var moveCurve = new Curve(moveTime, _options.TransitMoveCurve).CreateInstance();
            var heightCurve = new Curve(moveTime, _options.TransitHeightCurve).CreateInstance();
            var startPosition = _transform.WorldPosition;
            var directionSign = Mathf.Sign(target.position.x - _transform.WorldPosition.x);

            return _updater.RunUpdateAction(lifetime, IsCompleted, Move);

            void Move(float delta)
            {
                var moveFactor = moveCurve.StepForward(delta);
                var heightFactor = heightCurve.StepForward(delta);
                var height = Mathf.Lerp(0, _options.TransitHeight, heightFactor) * directionSign;
                var position = Vector2.Lerp(startPosition, target.position, moveFactor);
                position.x += height;
                _transform.SetWorldPosition(position);
            }

            bool IsCompleted()
            {
                if (moveCurve.IsFinished == true)
                    return false;
                
                distance = Vector2.Distance(_transform.WorldPosition, target.position);
                return distance > float.Epsilon;
            }
        }

        public UniTask FollowCursor(IReadOnlyLifetime lifetime, IPaintTarget from)
        {
            return _updater.RunUpdateAction(lifetime, delta =>
            {
                var targetPosition = _input.WorldPosition;
                var position = Vector2.Lerp(_transform.WorldPosition, targetPosition, _options.MoveSpeed * delta);
                _transform.SetWorldPosition(position);
            });
        }
    }
}