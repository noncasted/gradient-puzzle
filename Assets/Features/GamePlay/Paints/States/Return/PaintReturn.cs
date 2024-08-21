using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintReturn : IPaintReturn, IState
    {
        public PaintReturn(
            IUpdater updater,
            IStateMachine stateMachine,
            IPaintImage image,
            IPaintTransform transform,
            IPaintDrop drop,
            PaintReturnOptions options,
            PaintReturnDefinition definition)
        {
            _updater = updater;
            _stateMachine = stateMachine;
            _image = image;
            _transform = transform;
            _drop = drop;
            _options = options;
            Definition = definition;
        }

        private readonly IUpdater _updater;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintImage _image;
        private readonly IPaintTransform _transform;
        private readonly IPaintDrop _drop;
        private readonly PaintReturnOptions _options;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            Process(handle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            var startPosition = _transform.Position;
            var targetPosition = target.Position;

            await _updater.CurveProgression(lifetime, _options.Curve, progress =>
            {
                var position = Vector2.Lerp(startPosition, targetPosition, progress);
                _transform.SetPosition(position);
            });

            _drop.Enter(target);
        }
    }
}