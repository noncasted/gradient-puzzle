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

        private IPaintTarget _currentArea;
        private Vector2 _currentCenter;

        private MergeHandle _handle;
        private bool _showBody;

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

        public void Show(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets, bool showBody = true)
        {
            _showBody = showBody;
            _areas = targets;
            _body.SetColor(_sourceImage.Color);
            _fill.SetColor(_sourceImage.Color);
            _updater.Add(lifetime, this);
        }

        public void OnUpdate(float delta)
        {
            var (area, targetCenter) = GetClosestArea();

            if (area == null)
            {
                _currentArea = null;
                _currentCenter = Vector2.zero;
                _handle?.Dispose();
                _handle = null;

                return;
            }

            if (area == _currentArea)
            {
                var distance = Vector2.Distance(_transform.RectPosition, _currentCenter);

                if (area.IsInside(_transform.RectPosition) == false &&
                    _currentCenter != targetCenter &&
                    distance > _options.StartDistance)
                {
                    CreateHandle();
                }

                if (area.IsInside(_transform.RectPosition) == true)
                {
                    _handle?.UpdateCenter(targetCenter);
                    _currentCenter = targetCenter;
                }
            }
            else
            {
                if (_currentCenter != targetCenter)
                {
                    CreateHandle();
                }
            }

            _handle?.Update(delta);

            void CreateHandle()
            {
                _currentCenter = targetCenter;
                _currentArea = area;

                _handle?.Dispose();
                
                _handle = new MergeHandle(
                    _options,
                    _body,
                    _sourceImage,
                    _transform,
                    _fill,
                    area,
                    targetCenter,
                    _showBody);
            }
        }

        private (IPaintTarget, Vector2) GetClosestArea()
        {
            var minDistance = float.MaxValue;
            var targetArea = _areas[0];
            var targetCenter = Vector2.zero;

            foreach (var area in _areas)
            {
                foreach (var center in area.InnerPoints)
                {
                    var distance = Vector2.Distance(_transform.RectPosition, center);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        targetArea = area;
                        targetCenter = center;
                    }
                }
            }

            if (minDistance > _options.StartDistance)
                return (null, Vector2.zero);

            return (targetArea, targetCenter);
        }
    }
}