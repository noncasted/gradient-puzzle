using Common.StateMachines;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using Internal;

namespace GamePlay.Paints
{
    public class PaintReturn : IPaintReturn, IState
    {
        public PaintReturn(
            IStateMachine stateMachine,
            IPaintInterceptor interceptor,
            IPaintDrop drop,
            IPaintMover mover,
            IPaintMerging merging,
            IPaintTransform transform,
            PaintReturnDefinition definition)
        {
            _stateMachine = stateMachine;
            _interceptor = interceptor;
            _drop = drop;
            _mover = mover;
            _merging = merging;
            _transform = transform;
            Definition = definition;
        }

        private readonly IStateMachine _stateMachine;
        private readonly IPaintInterceptor _interceptor;
        private readonly IPaintDrop _drop;
        private readonly IPaintMover _mover;
        private readonly IPaintMerging _merging;
        private readonly IPaintTransform _transform;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            Process(handle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            target.PaintHandle.Lock();

            _merging.Show(new PaintMergingHandleOptions()
            {
                Lifetime = lifetime,
                ShowBody = false,
                ShowFill = false,
                Targets = new[] { target },
            });

            var nearestCenter = target.GetNearestCenter(_transform.RectPosition);
            await _mover.TransitTo(lifetime, nearestCenter);
            _interceptor.Attach(target);
            target.PaintHandle.Unlock();

            _drop.Enter(target);
        }
    }
}