using System;
using Cysharp.Threading.Tasks;
using Features.Services.Inputs;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
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

        public async UniTask TransitTo(IReadOnlyLifetime lifetime, Transform target)
        {
            var exitedStart = false;
            var moveLifetime = lifetime.Child();

            if (_interceptor.Current != null)
                MoveInsideTarget(_interceptor.Current).Forget();
            else
                exitedStart = true;
            
            var directionSign = Mathf.Sign(target.position.x - _transform.WorldPosition.x);

            var distance = Vector2.Distance(_transform.WorldPosition, target.position);
            var moveTime = _options.TransitTimeCurve.Evaluate(distance);
            var moveCurve = new Curve(moveTime, _options.TransitMoveCurve).CreateInstance();
            var heightCurve = new Curve(moveTime, _options.TransitHeightCurve).CreateInstance();
            var startPosition = _transform.WorldPosition;

            await _updater.RunUpdateAction(moveLifetime, IsMoved, delta =>
            {
                var moveFactor = moveCurve.Step(delta);
                var heightFactor = heightCurve.Step(delta);
                var height = Mathf.Lerp(0, _options.TransitHeight, heightFactor) * directionSign;
                var position = Vector2.Lerp(startPosition, target.position, moveFactor);
                position.x += height;
                _transform.SetWorldPosition(position);
            });

            moveLifetime.Terminate();
            return;

            async UniTask MoveInsideTarget(IPaintTarget paintTarget)
            {
                var startSize = _image.Size;
                var curve = paintTarget switch
                {
                    IArea => _options.AreaScaleCurve.CreateInstance(),
                    IPaintDock => _options.DockScaleCurve.CreateInstance(),
                    _ => throw new ArgumentOutOfRangeException(nameof(paintTarget))
                };

                await _updater.RunUpdateAction(
                    moveLifetime,
                    () => paintTarget.IsInside(_transform.RectPosition) && !curve.IsFinished,
                    delta =>
                    {
                        var factor = curve.Step(delta);
                        var size = Mathf.Lerp(startSize, _options.MoveSize, factor);
                        _image.SetSize(size);
                    });

                _image.ResetMaterial();
                _transform.AttachTo(_moveArea.Transform);
                _image.SetSize(_options.MoveSize);
                exitedStart = true;
            }

            bool IsMoved()
            {
                if (exitedStart == false)
                    return true;

                return moveCurve.IsFinished == false;
            }
        }

        public async UniTask FollowCursor(IReadOnlyLifetime lifetime, IPaintTarget from)
        {
            MoveInsideTarget(from).Forget();

            await _updater.RunUpdateAction(lifetime, delta =>
            {
                var targetPosition = _input.WorldPosition;
                var position = Vector2.Lerp(_transform.WorldPosition, targetPosition, _options.MoveSpeed * delta);
                _transform.SetWorldPosition(position);
            });

            return;

            async UniTask MoveInsideTarget(IPaintTarget paintTarget)
            {
                var startSize = _image.Size;
                var curve = paintTarget switch
                {
                    IArea => _options.AreaScaleCurve.CreateInstance(),
                    IPaintDock => _options.DockScaleCurve.CreateInstance(),
                    _ => throw new ArgumentOutOfRangeException(nameof(paintTarget))
                };

                await _updater.RunUpdateAction(
                    lifetime,
                    () => paintTarget.IsInside(_transform.RectPosition) && !curve.IsFinished,
                    delta =>
                    {
                        var factor = curve.Step(delta);
                        var size = Mathf.Lerp(startSize, _options.MoveSize, factor);
                        _image.SetSize(size);
                    });

                _image.ResetMaterial();
                _transform.AttachTo(_moveArea.Transform);
                _image.SetSize(_options.MoveSize);
            }
        }
    }
}