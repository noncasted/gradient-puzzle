using Common.StateMachines;
using Cysharp.Threading.Tasks;
using GamePlay.Common;

namespace GamePlay.Paints
{
    public class PaintAnchoring : IPaintAnchoring, IState
    {
        public PaintAnchoring(
            IPaintMover mover,
            IPaintInterceptor interceptor,
            IStateMachine stateMachine,
            IPaintDrop drop,
            IPaintMerging merging,
            IPaintTransform transform,
            PaintAnchoringDefinition definition)
        {
            _mover = mover;
            _interceptor = interceptor;
            _stateMachine = stateMachine;
            _drop = drop;
            _merging = merging;
            _transform = transform;
            Definition = definition;
        }

        private readonly IPaintMover _mover;
        private readonly IPaintInterceptor _interceptor;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintDrop _drop;
        private readonly IPaintMerging _merging;
        private readonly IPaintTransform _transform;

        public IStateDefinition Definition { get; }

        public async UniTask Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);

            _interceptor.Detach();
            _interceptor.Attach(target);

            _merging.Show(new PaintMergingHandleOptions()
            {
                Lifetime = handle.Lifetime,
                Targets = new[] { target },
                ShowBody = false,
            });

            await _mover.TransitTo(handle.Lifetime, target.GetNearestCenter(_transform.RectPosition));
            await _drop.Enter(target);
        }
    }
}