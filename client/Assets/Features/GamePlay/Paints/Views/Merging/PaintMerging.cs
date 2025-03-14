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
        private AreaCenter _currentCenter;

        private MergeHandle _handle;

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
                _currentCenter = null;
                _handle?.Dispose();
                _handle = null;

                return;
            }

            if (area == _currentArea)
            {
                if (targetCenter != _currentCenter)
                {
                    var distance = Vector2.Distance(_transform.WorldPosition, _currentCenter.Transform.position);

                    if (distance < _options.StartDistance)
                        targetCenter = _currentCenter;
                }
            }

            if (_currentCenter != targetCenter)
                _handle = new MergeHandle(_options, _body, _sourceImage, _transform, _fill, area, targetCenter);

            _currentCenter = targetCenter;
            _currentArea = area;

            _handle.Update(delta);
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