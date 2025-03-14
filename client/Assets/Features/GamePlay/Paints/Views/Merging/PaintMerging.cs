using System.Collections.Generic;
using GamePlay.Common;
using Global.Systems;
using Internal;
using UnityEngine;
using VContainer;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintMerging : MonoBehaviour, IPaintMerging, IUpdatable, IEntityComponent
    {
        [SerializeField] private PaintMergingOptions _options;
        [SerializeField] private PaintMergingBody _body;

        private IPaintImage _sourceImage;
        private IUpdater _updater;
        private IReadOnlyList<IPaintTarget> _areas;
        private IPaintTransform _transform;
        private IPaintFill _fill;

        private float _validSize;
        private IPaintTarget _currentArea;
        private AreaCenter _currentCenter;

        [Inject]
        private void Construct(
            IUpdater updater,
            IPaintImage sourceImage,
            IPaintTransform transform,
            IPaintFill fill)
        {
            _fill = fill;
            _transform = transform;
            _updater = updater;
            _sourceImage = sourceImage;
        }

        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintMerging>();
        }

        public void Show(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets)
        {
            _areas = targets;
            _body.Setup(_sourceImage.Color);
            _fill.SetColor(_sourceImage.Color);
            _updater.Add(lifetime, this);
        }

        public void OnUpdate(float delta)
        {
            var (area, targetCenter) = GetClosestArea();

            if (area == null)
            {
                _currentArea = null;
                _currentCenter = null;
                
                _body.UpdatePath(new List<Vector2>());
                _fill.ResetMaterial();
                _fill.SetRectPosition(Vector2.zero);
                _fill.SetSize(0);
                _validSize = 0;
                return;
            }

            if (area != _currentArea)
                _validSize = 0;

            if (area == _currentArea)
            {
                if (targetCenter != _currentCenter)
                {
                    var distance = Vector2.Distance(_transform.WorldPosition, _currentCenter.Transform.position);

                    if (distance < _options.StartDistance)
                        targetCenter = _currentCenter;
                }
            }

            _currentCenter = targetCenter;
            _currentArea = area;

            _transform.AttachTo(area.SelfTransform);

            if (area.IsInside(_transform.RectPosition) == true)
                _body.SetMaterial(area.MaskData?.Content);
            else
                _body.SetMaterial(null);

            var targetSizeRange = new Vector2(10, targetCenter.Size);

            var paintWorldCenter = _transform.WorldPosition;

            var direction = ((Vector2)targetCenter.Transform.position - _transform.WorldPosition).normalized;
            var distanceToArea = Vector2.Distance(targetCenter.Transform.position, _transform.WorldPosition);
            var moveProgress = 1f - Mathf.Clamp01(distanceToArea / _options.StartDistance);

            var targetSize = Mathf.Lerp(
                targetSizeRange.x,
                targetSizeRange.y,
                _options.TargetSizeCurve.Evaluate(moveProgress));

            var targetPositionFactor = _options.TargetPositionCurve.Evaluate(moveProgress);

            if (targetPositionFactor >= 1f)
                _fill.SetMaterial(area.MaskData?.Content);
            else
                _fill.ResetMaterial();

            var targetPosition = Vector2.Lerp(
                paintWorldCenter,
                targetCenter.Transform.position,
                targetPositionFactor);

            _fill.SetWorldPosition(new Vector2(targetPosition.x, targetPosition.y));
            _fill.SetSize(targetSize);
            
            if (distanceToArea < 0.1f)
                return;

            var middlePointPositionFactor = _options.MiddlePointPositionCurve.Evaluate(moveProgress);

            var middlePointPosition = Vector2.Lerp(
                Vector2.zero,
                _fill.RectPosition,
                middlePointPositionFactor);

            var circlePointOffset = (new Angle(direction.ToAngle() + 90f)).ToVector2() * 0.5f;

            var pathB = _fill.RectPosition + circlePointOffset * targetSize;
            var pathC = _fill.RectPosition + circlePointOffset * -targetSize;

            var checkB = _transform.RectPosition + pathB;
            var checkC = _transform.RectPosition + pathC;

            if (area.IsInside(checkB) == true && area.IsInside(checkC) == true)
            {
                _validSize = targetSize;
            }
            else
            {
                pathB = _fill.RectPosition + circlePointOffset * _validSize;
                pathC = _fill.RectPosition + circlePointOffset * -_validSize;
            }

            var middlePointHeight = _options.MiddlePointHeightCurve.Evaluate(moveProgress) * _validSize;

            var path = new List<Vector2>();

            var pathA = circlePointOffset * _sourceImage.Size;

            var pathAB = middlePointPosition + circlePointOffset * middlePointHeight;

            var pathCD = middlePointPosition + circlePointOffset * -middlePointHeight;

            var pathD = circlePointOffset * -_sourceImage.Size;

            path.Add(pathA);

            path.AddRange(CalculateQuadraticBezier(pathA, pathAB, pathB));

            path.Add(pathB);
            path.Add(pathC);

            path.AddRange(CalculateQuadraticBezier(pathC, pathCD, pathD));

            path.Add(pathD);

            _body.UpdatePath(path);
        }

        private List<Vector2> CalculateQuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            var points = new List<Vector2>();

            for (float t = 0; t <= 1; t += _options.Step)
            {
                var oneMinusT = 1 - t;
                var point = (oneMinusT * oneMinusT * p0) +
                            (2 * oneMinusT * t * p1) +
                            (t * t * p2);
                points.Add(point);
            }

            return points;
        }

        private (IPaintTarget, AreaCenter) GetClosestArea()
        {
            var minDistance = float.MaxValue;
            var targetArea = _areas[0];
            var targetCenter = _areas[0].Centers[0];

            foreach (var area in _areas)
            {
                foreach (var center in area.Centers)
                {
                    var distance = Vector2.Distance(_transform.WorldPosition, center.Transform.position);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetArea = area;
                        targetCenter = center;
                    }
                }
            }

            if (minDistance > _options.StartDistance)
                return (null, null);
            

            return (targetArea, targetCenter);
        }
    }
}