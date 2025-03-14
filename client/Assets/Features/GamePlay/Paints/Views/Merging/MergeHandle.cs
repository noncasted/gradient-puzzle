using System.Collections.Generic;
using GamePlay.Common;
using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    public class MergeHandle
    {
        public MergeHandle(
            PaintMergingOptions options,
            PaintMergingBody body,
            IPaintImage sourceImage,
            IPaintTransform transform,
            IPaintFill fill,
            IPaintTarget area,
            AreaCenter center)
        {
            _options = options;
            _body = body;
            _sourceImage = sourceImage;
            _transform = transform;
            _fill = fill;
            _area = area;
            _center = center;
        }

        private readonly PaintMergingOptions _options;
        private readonly PaintMergingBody _body;
        private readonly IPaintImage _sourceImage;
        private readonly IPaintTransform _transform;
        private readonly IPaintFill _fill;

        private readonly IPaintTarget _area;
        private readonly AreaCenter _center;

        private readonly List<UIVertex> _bodyPath = new();

        private float _validSize;
        private float _moveProgress;

        public void Update(float delta)
        {
            _transform.AttachTo(_area.SelfTransform);

            if (_area.IsInside(_transform.RectPosition) == true)
                _body.SetMaterial(_area.MaskData?.Content);
            else
                _body.SetMaterial(null);

            var distanceToArea = Vector2.Distance(_center.Transform.position, _transform.WorldPosition);
            _moveProgress = 1f - Mathf.Clamp01(distanceToArea / _options.StartDistance);

            var targetSizeRange = new Vector2(_options.MinFillSize, _center.Size);

            var targetSize = Mathf.Lerp(
                targetSizeRange.x,
                targetSizeRange.y,
                _options.TargetSizeCurve.Evaluate(_moveProgress));

            var targetPositionFactor = _options.TargetPositionCurve.Evaluate(_moveProgress);

            if (targetPositionFactor >= 1f)
                _fill.SetMaterial(_area.MaskData?.Content);
            else
                _fill.ResetMaterial();

            var paintWorldCenter = _transform.WorldPosition;
            var targetPosition = Vector2.Lerp(
                paintWorldCenter,
                _center.Transform.position,
                targetPositionFactor);

            _fill.SetWorldPosition(new Vector2(targetPosition.x, targetPosition.y));
            _fill.SetSize(targetSize);

            if (distanceToArea < 0.1f)
                return;


            _body.UpdatePath(this);
        }

        public List<UIVertex> GetBodyPath()
        {
            _bodyPath.Clear();

            var targetSizeRange = new Vector2(_options.MinFillSize, _center.Size);

            var targetSize = Mathf.Lerp(
                targetSizeRange.x,
                targetSizeRange.y,
                _options.TargetSizeCurve.Evaluate(_moveProgress));

            var direction = ((Vector2)_center.Transform.position - _transform.WorldPosition).normalized;
            var circlePointOffset = (new Angle(direction.ToAngle() + 90f)).ToVector2() * 0.5f;

            var validSize = GetValidTargetSize();
            _validSize = validSize;

            var pathB = _fill.RectPosition + circlePointOffset * validSize;
            var pathC = _fill.RectPosition + circlePointOffset * -validSize;

            var middlePointHeight = _options.MiddlePointHeightCurve.Evaluate(_moveProgress) * validSize;
            var middlePointPositionFactor = _options.MiddlePointPositionCurve.Evaluate(_moveProgress);

            var middlePointPosition = Vector2.Lerp(
                Vector2.zero,
                _fill.RectPosition,
                middlePointPositionFactor);

            var pathA = circlePointOffset * _sourceImage.Size;
            var pathAB = middlePointPosition + circlePointOffset * middlePointHeight;
            var pathCD = middlePointPosition + circlePointOffset * -middlePointHeight;
            var pathD = circlePointOffset * -_sourceImage.Size;

            AddPoint(pathA);
            CalculateQuadraticBezier(pathA, pathAB, pathB);
            AddPoint(pathB);
            AddPoint(pathC);
            CalculateQuadraticBezier(pathC, pathCD, pathD);
            AddPoint(pathD);

            return _bodyPath;

            void CalculateQuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2)
            {
                for (float t = 0; t <= 1; t += _options.Step)
                {
                    var oneMinusT = 1 - t;
                    var point = (oneMinusT * oneMinusT * p0) +
                                (2 * oneMinusT * t * p1) +
                                (t * t * p2);
                    AddPoint(point);
                }
            }

            void AddPoint(Vector2 point)
            {
                var vertex = UIVertex.simpleVert;
                vertex.position = point;
                _bodyPath.Add(vertex);
            }

            float GetValidTargetSize()
            {
                var fillPosition = _transform.RectPosition + _fill.RectPosition;

                if (_area.IsInside(fillPosition) == false)
                    return targetSize;

                var checkSize = targetSize;
                var step = checkSize * 0.1f;

                var checkB = fillPosition + circlePointOffset * checkSize;
                var checkC = fillPosition + circlePointOffset * -checkSize;

                while (IsInside() == false && checkSize > 0)
                {
                    checkSize -= step;

                    checkB = fillPosition + circlePointOffset * checkSize;
                    checkC = fillPosition + circlePointOffset * -checkSize;
                }

                if (checkSize < 0)
                    checkSize = _options.MinFillSize;

                return checkSize;

                bool IsInside()
                {
                    return _area.IsInside(checkB) == true && _area.IsInside(checkC) == true;
                }
            }
        }

        public void Dispose()
        {
            _body.UpdatePath(null);
            _fill.ResetMaterial();
            _fill.SetRectPosition(Vector2.zero);
            _fill.SetSize(0);
            _validSize = 0;
        }
    }
}