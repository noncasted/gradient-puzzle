using System;
using Cysharp.Threading.Tasks;
using Global.Systems;
using Internal;
using Services;
using UnityEngine;
using VContainer;

namespace GamePlay.Paints
{
    public class PaintTestState : MonoBehaviour, IEntityComponent
    {
        [SerializeField] private Vector2 _range;
        [SerializeField] private float _speed;
        
        private IPaintMoveArea _moveArea;
        private IPaintTransform _transform;
        private IUpdater _updater;
        private IPaintMerging _merging;
        private IGameContext _gameContext;

        [Inject]
        private void Construct(
            IUpdater updater,
            IPaintMoveArea moveArea,
            IPaintTransform transform,
            IPaintMerging merging,
            IGameContext gameContext)
        {
            _gameContext = gameContext;
            _merging = merging;
            _updater = updater;
            _moveArea = moveArea;
            _transform = transform;
        }
        
        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .AsSelfResolvable();
        }

        public async UniTask Run(IReadOnlyLifetime lifetime)
        {
            _transform.AttachTo(_moveArea.Transform);
            _transform.SetRectPosition(new Vector2(0f, _range.x));
            
            _merging.Show(lifetime, _gameContext.Targets);
        }

        private void Update()
        {
            return;
            var factor = Mathf.Sin(Time.time * _speed);
            var y = Mathf.Lerp(_range.x, _range.y, (factor + 1f) / 2f);
            
            _transform.SetRectPosition(new Vector2(0f, y));
        }
    }
}