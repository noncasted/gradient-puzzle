using System.Collections.Generic;
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
        [SerializeField] private AreaCenter[] _centers;

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
        public IReadOnlyList<AreaCenter> Centers => _centers;
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
            _centerTransform.anchoredPosition = AreaShapeDataExtensions.GetCenter(shapes);

            _color = color;
            _renderer.SetColor(color);
        }

        public void UpdateShapes(IReadOnlyList<AreaShapeData> shapes)
        {
            _shapes = new List<AreaShapeData>(shapes);
            _renderer.Construct(shapes);
            _renderer.SetColor(_color);
        }

        public void Setup(Color color, RenderMaskData maskData, Transform outlineParent)
        {
            _centers = GetComponentsInChildren<AreaCenter>(true);
            _centerTransform.SetAsLastSibling();
            _color = _renderer.Color;
            _maskData = maskData;
            _renderer.SetMaterial(maskData.Area);
            _renderer.SetColor(color);

            var lifetime = this.GetObjectLifetime();

            // foreach (var areaRenderer in _renderer.Renderers)
            // {
            //     areaRenderer.Outline.transform.parent = outlineParent;
            //     areaRenderer.Outline.transform.SetAsLastSibling();
            // }

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

        public float GetMinDistanceToBorder(Vector2 position)
        {
            var minDistance = float.MaxValue;

            foreach (var data in _shapes)
            {
                var distance = data.GetMinDistanceToBorder(position);

                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
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