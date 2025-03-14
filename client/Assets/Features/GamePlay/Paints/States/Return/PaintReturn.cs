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
            PaintReturnDefinition definition)
        {
            _stateMachine = stateMachine;
            _interceptor = interceptor;
            _drop = drop;
            _mover = mover;
            _merging = merging;
            Definition = definition;
        }

        private readonly IStateMachine _stateMachine;
        private readonly IPaintInterceptor _interceptor;
        private readonly IPaintDrop _drop;
        private readonly IPaintMover _mover;
        private readonly IPaintMerging _merging;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            Process(handle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            target.PaintHandle.Lock();
            _merging.Show(lifetime, target);
            await _mover.TransitTo(lifetime, target.RootCenter, _interceptor.Current);
            _interceptor.Attach(target);
            target.PaintHandle.Unlock();

            _drop.Enter(target);
        }
    }
}