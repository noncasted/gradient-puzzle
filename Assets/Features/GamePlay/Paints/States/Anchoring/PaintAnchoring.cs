using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Global.Systems;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public class PaintAnchoring : IPaintAnchoring, IState
    {
        public PaintAnchoring(
            IUpdater updater,
            IStateMachine stateMachine,
            IPaintImage image,
            IPaintTransform transform,
            IPaintMoveArea moveArea,
            IPaintDrop drop,
            PaintAnchoringOptions options,
            PaintAnchoringDefinition definition)
        {
            Definition = definition;
            _updater = updater;
            _stateMachine = stateMachine;
            _image = image;
            _transform = transform;
            _moveArea = moveArea;
            _drop = drop;
            _options = options;
        }

        private readonly IUpdater _updater;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintImage _image;
        private readonly IPaintTransform _transform;
        private readonly IPaintMoveArea _moveArea;
        private readonly IPaintDrop _drop;
        private readonly PaintAnchoringOptions _options;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var stateHandle = _stateMachine.CreateHandle(this);
            Process(stateHandle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            await ScaleDown();
            
            _transform.AttachTo(_moveArea.Transform);

            var shouldMove = true;
            await _updater.RunUpdateAction(lifetime, () => shouldMove, Move);
            _drop.Enter(target);

            return;

            async UniTask ScaleDown()
            {
                var curve = _options.DragSizeCurve.CreateInstance();
                var startSize = _image.Size;

                await _updater.RunUpdateAction(lifetime, () => !curve.IsFinished, delta =>
                {
                    var factor = curve.Step(delta);
                    var size = Mathf.Lerp(startSize, _options.DragSize, factor);
                    _image.SetSize(size);
                });
            }

            void Move(float delta)
            {
                var targetPosition = target.Position;
                var currentPosition = _transform.Position;

                var move = _options.MoveSpeed * delta;
                var position = Vector2.Lerp(currentPosition, targetPosition, move);

                _transform.SetPosition(position);

                var distance = Vector2.Distance(position, targetPosition);
                
                if (distance < _options.DistanceThreshold)
                    shouldMove = false;
            }
        }
    }
}