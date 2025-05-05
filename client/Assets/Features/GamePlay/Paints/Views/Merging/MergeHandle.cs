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
            Vector2 center,
            PaintMergingHandleOptions handleOptions)
        {
            _options = options;
            _body = body;
            _sourceImage = sourceImage;
            _transform = transform;
            _fill = fill;
            _area = area;
            _center = center;
            _showBody = handleOptions.ShowBody;

            _currentCenter = center;
        }

        private readonly PaintMergingOptions _options;
        private readonly PaintMergingBody _body;
        private readonly IPaintImage _sourceImage;
        private readonly IPaintTransform _transform;
        private readonly IPaintFill _fill;

        private readonly IPaintTarget _area;
        private readonly List<UIVertex> _bodyPath = new();
        private readonly bool _showBody;

        private Vector2 _center;
        private Vector2 _currentCenter;
        private float _moveProgress;
        private float _timer;
        private float _initTimer;

        public void UpdateCenter(Vector2 center)
        {
            _center = center;
        }

        public void Update(float delta)
        {
            _initTimer += delta;
            
            if (_initTimer < _options.InitTime)
                return;
            
            _timer += delta;
            _transform.AttachTo(_area.SelfTransform);

            if (_area.IsInside(_transform.RectPosition) == true)
            {
                _body.SetMaterial(_area.MaskData?.Content);
            }
            else
            {
                _body.SetMaterial(null);
            }

            _currentCenter = _center;
            var distanceToArea = Vector2.Distance(_currentCenter, _transform.RectPosition);
            _moveProgress = 1f - Mathf.Clamp01(distanceToArea / _options.StartDistance);

            // if (_area.IsInside(_transform.RectPosition) == true)
            //     _moveProgress = Mathf.Max(_moveProgress, _insideTimer / _options.Time);

            _moveProgress = Mathf.Clamp(_moveProgress, 0f, _timer / _options.Time);
            var targetSizeRange = new Vector2(_options.MinFillSize, _area.Size * 4f);

            var targetSize = Mathf.Lerp(
                targetSizeRange.x,
                targetSizeRange.y,
                _options.TargetSizeCurve.Evaluate(_moveProgress));

            var targetPositionFactor = _options.TargetPositionCurve.Evaluate(_moveProgress);

            if (targetPositionFactor >= 1f)
                _fill.SetMaterial(_area.MaskData?.Content);
            else
                _fill.ResetMaterial();

            _fill.SetSize(targetSize);

            if (_area.IsInside(_transform.RectPosition) == false && _showBody == true)
            {
                var fillPosition = Vector2.Lerp(
                    _transform.RectPosition,
                    _currentCenter,
                    targetPositionFactor) - _transform.RectPosition;

                _fill.SetRectPosition(fillPosition);
                _body.UpdatePath(this);
            }
            else
            {
                _fill.SetRectPosition(Vector2.zero);
                _body.UpdatePath(null);
            }
        }

        public List<UIVertex> GetBodyPath()
        {
            _bodyPath.Clear();

            var targetSizeRange = new Vector2(_options.MinFillSize, _area.Size * 3f);

            var targetSize = Mathf.Lerp(
                targetSizeRange.x,
                targetSizeRange.y,
                _options.TargetSizeCurve.Evaluate(_moveProgress));

            targetSize = Mathf.Clamp(targetSize, 0, _area.Size);

            var direction = (_currentCenter - _transform.RectPosition).normalized;
            var circlePointOffset = (new Angle(direction.ToAngle() + 90f)).ToVector2() * 0.5f;

            var (validSizeB, validSizeC) = GetValidTargetSize();

            var pathB = _fill.RectPosition + circlePointOffset * validSizeB;
            var pathC = _fill.RectPosition + circlePointOffset * -validSizeC;

            var middlePointHeightB = _options.MiddlePointHeightCurve.Evaluate(_moveProgress) * validSizeB;
            var middlePointHeightC = _options.MiddlePointHeightCurve.Evaluate(_moveProgress) * validSizeC;
            var middlePointPositionFactor = _options.MiddlePointPositionCurve.Evaluate(_moveProgress);

            var middlePointPosition = Vector2.Lerp(
                Vector2.zero,
                _fill.RectPosition,
                middlePointPositionFactor);

            var pathA = circlePointOffset * _sourceImage.Size;
            var pathAB = middlePointPosition + circlePointOffset * middlePointHeightB;
            var pathCD = middlePointPosition + circlePointOffset * -middlePointHeightC;
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

            (float, float) GetValidTargetSize()
            {
                var fillPosition = _transform.RectPosition + _fill.RectPosition;

                if (_area.IsInside(fillPosition) == false)
                {
                    var size = Mathf.Min(targetSize, _options.MinFillSize);
                    return (size, size);
                }

                var checkSizeB = 0f;
                var checkSizeC = 0f;
                var step = 1;
                var safeGuard = 0;

                var checkB = fillPosition + circlePointOffset * checkSizeB;
                var checkC = fillPosition + circlePointOffset * -checkSizeC;

                while (_area.IsInside(checkB) == true && checkSizeB < targetSize && safeGuard < 100)
                {
                    safeGuard++;
                    checkSizeB += step;

                    checkB = fillPosition + circlePointOffset * checkSizeB;
                }

                safeGuard = 0;

                while (_area.IsInside(checkC) == true && checkSizeC < targetSize && safeGuard < 100)
                {
                    safeGuard++;
                    checkSizeC += step;

                    checkC = fillPosition + circlePointOffset * -checkSizeC;
                }

                if (_area.IsInside(checkB) == false)
                    checkSizeB -= step;

                if (_area.IsInside(checkC) == false)
                    checkSizeC -= step;

                if (checkSizeB < 0)
                    checkSizeB = _options.MinFillSize;

                if (checkSizeC < 0)
                    checkSizeC = _options.MinFillSize;

                return (checkSizeB, checkSizeC);
            }
        }

        public void Dispose()
        {
            _body.UpdatePath(null);
            _fill.ResetMaterial();
            _fill.SetRectPosition(Vector2.zero);
            _fill.SetSize(0);
        }
    }
}