using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;
using Internal;

namespace Features.GamePlay
{
    public class PaintReturn : IPaintReturn, IState
    {
        public PaintReturn(
            IStateMachine stateMachine,
            IPaintInterceptor interceptor,
            IPaintDrop drop,
            IPaintMover mover,
            PaintReturnDefinition definition)
        {
            _stateMachine = stateMachine;
            _interceptor = interceptor;
            _drop = drop;
            _mover = mover;
            Definition = definition;
        }

        private readonly IStateMachine _stateMachine;
        private readonly IPaintInterceptor _interceptor;
        private readonly IPaintDrop _drop;
        private readonly IPaintMover _mover;

        public IStateDefinition Definition { get; }

        public void Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            Process(handle.Lifetime, target).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime, IPaintTarget target)
        {
            target.PaintHandle.Lock();
            await _mover.TransitTo(lifetime, target.CenterTransform);
            _interceptor.Attach(target);
            target.PaintHandle.Unlock();
            
            _drop.Enter(target);
        }
    }
}