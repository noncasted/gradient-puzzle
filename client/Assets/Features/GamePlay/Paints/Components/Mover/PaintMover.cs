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

        public async UniTask TransitTo(IReadOnlyLifetime lifetime, Transform target, IPaintTarget from)
        {
            var distance = Vector2.Distance(_transform.WorldPosition, target.position);
            var moveTime = _options.TransitTimeCurve.Evaluate(distance);
            var moveCurve = new Curve(moveTime, _options.TransitMoveCurve).CreateInstance();
            var heightCurve = new Curve(moveTime, _options.TransitHeightCurve).CreateInstance();
            var startPosition = _transform.WorldPosition;
            var directionSign = Mathf.Sign(target.position.x - _transform.WorldPosition.x);

            var moveTask = _updater.RunUpdateAction(lifetime, IsCompleted, Move);

            if (from != null)
            {
                var scaleTask = ScaleDown(lifetime, from);
                await UniTask.WhenAll(moveTask, scaleTask);
                return;
            }

            await moveTask;
            
            return;

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

        public async UniTask FollowCursor(IReadOnlyLifetime lifetime, IPaintTarget from)
        {
            ScaleDown(lifetime, from).Forget();

            await _updater.RunUpdateAction(lifetime, delta =>
            {
                var targetPosition = _input.WorldPosition;
                var position = Vector2.Lerp(_transform.WorldPosition, targetPosition, _options.MoveSpeed * delta);
                _transform.SetWorldPosition(position);
            });
        }

        private async UniTask ScaleDown(IReadOnlyLifetime lifetime, IPaintTarget from)
        {
            var curve = GetScaleCurve(from);
            var curveTime = curve.Time;
            var curveTimer = 0f;
            var progress = 0f;
            var startDistance = from.GetMinDistanceToBorder(_transform.RectPosition);
            var imageSize = _image.Size;

            await _updater.RunUpdateAction(lifetime, () => progress < 1f, delta =>
            {
                curveTimer += delta;
                progress = GetProgress();
                var factor = curve.Evaluate(progress);
                var size = Mathf.Lerp(imageSize, _options.MoveSize, factor);
                _image.SetSize(size);
            });

            _image.ResetMaterial();
            _transform.AttachTo(_moveArea.Transform);
            _image.SetSize(_options.MoveSize);

            return;

            float GetProgress()
            {
                var borderProgress = GetBorderProgress();
                var curveProgress = curveTimer / curveTime;

                var result = Mathf.Max(borderProgress, curveProgress);

                return Mathf.Max(result, progress);

                float GetBorderProgress()
                {
                    if (from.IsInside(_transform.RectPosition) == false)
                        return 1f;

                    var borderDistance = from.GetMinDistanceToBorder(_transform.RectPosition);
                    return Mathf.Clamp01(1f - (borderDistance / startDistance));
                }
            }
        }

        private Curve GetScaleCurve(IPaintTarget target)
        {
            return target switch
            {
                IArea area => _options.AreaScaleCurve,
                IPaintDock dock => _options.DockScaleCurve,
                _ => throw new ArgumentOutOfRangeException(nameof(target))
            };
        }
    }
}