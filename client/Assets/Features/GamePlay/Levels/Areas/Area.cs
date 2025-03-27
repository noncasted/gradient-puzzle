using System.Collections.Generic;
using System.Linq;
using GamePlay.Common;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Levels
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Area : MonoBehaviour, IArea
    {
        [SerializeField] private bool _isAnchor;
        [SerializeField] private AreaRenderer _renderer;

        [SerializeField] private Color _color;
        [SerializeField] private int _order;
        [SerializeField] private string _id;

        [SerializeField] private RectTransform _selfTransform;
        [SerializeField] private RectTransform _centerTransform;

        [SerializeField] private List<AreaShapeData> _shapes;
        [SerializeField] private List<Vector2> _innerPoints;
        [SerializeField] private float _size;

        private readonly ViewableProperty<bool> _isTouched = new();
        private readonly ViewableProperty<bool> _isCompleted = new();

        private RenderMaskData _maskData;

        public IViewableProperty<bool> IsTouched => _isTouched;
        public IViewableProperty<bool> IsCompleted => _isCompleted;

        public Vector2 Position => _centerTransform.position;
        public RectTransform SelfTransform => _selfTransform;
        public RectTransform RootCenter => _centerTransform;

        public bool IsAnchor => _isAnchor;
        public RenderMaskData MaskData => _maskData;
        public IPaintHandle PaintHandle { get; } = new PaintHandle();
        public IReadOnlyList<Vector2> InnerPoints => _innerPoints;
        public float Size => _size;
        public Color Color => _color;
        public AreaRenderer Renderer => _renderer;
        public IReadOnlyList<AreaShapeData> Shapes => _shapes;
        public int Order => _order;
        public string Id => _id;

        public void Construct(
            IReadOnlyList<AreaShapeData> shapes,
            Color color,
            int order,
            string id)
        {
            _id = id;
            _order = order;
            _shapes = new List<AreaShapeData>(shapes);
            _renderer.Construct(shapes);

            _color = color;
            _renderer.SetColor(color);
        }

        public void RecalculateInnerPoints()
        {
            _innerPoints = new List<Vector2>();

            foreach (var shape in _shapes)
            {
                foreach (var point in shape.InnerPoint)
                    _innerPoints.Add(point);
            }

            foreach (var shapeA in _shapes)
            {
                foreach (var shapeB in _shapes)
                {
                    foreach (var pointA in shapeA.SystemPoints)
                    {
                        foreach (var pointB in shapeB.SystemPoints)
                        {
                            var distance = Vector2.Distance(pointA, pointB);

                            if (distance > _size)
                                _size = distance;
                        }
                    }
                }
            }

            _centerTransform.anchoredPosition = _innerPoints.GetCenter();

            if (IsInside(_centerTransform.anchoredPosition) == false)
                _centerTransform.anchoredPosition = _innerPoints.First();
        }

        public void UpdateShapes(IReadOnlyList<AreaShapeData> shapes)
        {
            _shapes = new List<AreaShapeData>(shapes);
            _renderer.Construct(shapes);
            _renderer.SetColor(_color);
        }

        public void Setup(Color color, RenderMaskData maskData, Transform outlineParent)
        {
            _centerTransform.SetAsLastSibling();
            _color = _renderer.Color;
            _maskData = maskData;
            _renderer.SetMaterial(maskData.Area);
            _renderer.SetColor(color);

            var lifetime = this.GetObjectLifetime();

            foreach (var shapeA in _shapes)
            {
                foreach (var shapeB in _shapes)
                {
                    foreach (var pointA in shapeA.SystemPoints)
                    {
                        foreach (var pointB in shapeB.SystemPoints)
                        {
                            var distance = Vector2.Distance(pointA, pointB);

                            if (distance > _size)
                                _size = distance;
                        }
                    }
                }
            }

            PaintHandle.Paint.Advise(lifetime, paint =>
            {
                if (paint == null)
                {
                    foreach (var areaRenderer in _renderer.Renderers)
                        areaRenderer.Outline.Enable();

                    _isCompleted.Set(false);
                    return;
                }

                foreach (var areaRenderer in _renderer.Renderers)
                    areaRenderer.Outline.Disable();

                _isCompleted.Set(paint.Color == _color);
            });
        }

        public bool IsInside(Vector2 position)
        {
            return CheckInside(position);
        }


        public bool CheckTouch(Vector2 cursorPosition)
        {
            if (_isAnchor == true)
                return false;

            var isInside = CheckInside(cursorPosition);

            _isTouched.Set(isInside);
            return isInside;
        }

        public void ResetTouch()
        {
            _isTouched.Set(false);
        }

        private bool CheckInside(Vector2 position)
        {
            foreach (var data in _shapes)
            {
                if (data.IsInside(position) == false)
                    continue;

                return true;
            }

            return false;
        }
    }
}