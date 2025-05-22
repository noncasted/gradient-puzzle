using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Selections;
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
            IPaintMoveArea moveArea,
            IPaintFill fill)
        {
            _options = options;
            _body = body;
            _sourceImage = sourceImage;
            _transform = transform;
            _moveArea = moveArea;
            _fill = fill;
        }

        private readonly PaintMergingOptions _options;
        private readonly PaintMergingBody _body;
        private readonly IPaintImage _sourceImage;
        private readonly IPaintTransform _transform;
        private readonly IPaintMoveArea _moveArea;
        private readonly IPaintFill _fill;

        private readonly List<UIVertex> _bodyPath = new();

        private IPaintTarget _area;
        private bool _showBody;
        private float _moveProgress;
        private float _timer;

        private Vector2 _targetCenter;
        private Vector2 _currentCenter;

        public void UpdateCenter(Vector2 center)
        {
            _targetCenter = center;
            _currentCenter = center;
        }

        public void SetBody(bool showBody)
        {
            _showBody = showBody;

            if (showBody == false)
                _body.UpdatePath(null);
        }

        public void SetArea(IPaintTarget area)
        {
            _area = area;
            _body.SetColor(_sourceImage.Color);
            _fill.SetColor(_sourceImage.Color);
            _timer = 0f;
        }

        public void Update(float delta)
        {
            if (_showBody == false || _area.PaintHandle.Paint.Value != null)
                _body.UpdatePath(null);

            _timer += delta;

            if (_area.IsInside(_transform.RectPosition) == true)
                _body.SetMaterial(_area.MaskData?.Content);
            else
                _body.SetMaterial(null);

            RecalculateTarget();

            _currentCenter = Vector2.Lerp(_currentCenter, _targetCenter, delta * _options.CenterMoveSpeed);

            var distanceToArea = Vector2.Distance(_currentCenter, _transform.RectPosition);
            var targetProgress = 1f - Mathf.Clamp01(distanceToArea / _options.StartDistance);

            if (distanceToArea <= _options.StartDistance)
                _transform.AttachTo(_area.SelfTransform);
            else
                _transform.AttachTo(_moveArea.Transform);


            if (_area.IsInside(_transform.RectPosition) == true)
                targetProgress = 1f;

            _moveProgress = Mathf.Lerp(_moveProgress, targetProgress, delta * _options.ProgressSpeed);
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

            if (_area.IsInside(_transform.RectPosition) == false)
                _fill.SetVisible(_area.PaintHandle.Paint.Value == null);
            else
                _fill.SetVisible(true);

            if (_showBody == true)
            {
                var fillPosition = Vector2.Lerp(
                    _transform.RectPosition,
                    _currentCenter,
                    targetPositionFactor) - _transform.RectPosition;

                _fill.SetRectPosition(fillPosition);

                if (_area.PaintHandle.Paint.Value == null)
                    _body.UpdatePath(this);
            }
        }

        private void RecalculateTarget()
        {
            if (_area is IPaintDock)
                return;

            var firstNearest = _area.GetNearestCenter(_transform.RectPosition);
            var secondNearest = _area.GetNearestCenter(_transform.RectPosition, firstNearest);

            var distanceToFirst = Vector2.Distance(_transform.RectPosition, firstNearest);
            var distanceToSecond = Vector2.Distance(_transform.RectPosition, secondNearest);

            if (distanceToFirst < distanceToSecond)
            {
                var lerp = distanceToFirst / distanceToSecond;
                _targetCenter = Vector2.Lerp(firstNearest, secondNearest, lerp);
            }
            else
            {
                var lerp = distanceToSecond / distanceToFirst;
                _targetCenter = Vector2.Lerp(secondNearest, firstNearest, lerp);
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
    }
}