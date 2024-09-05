using Cysharp.Threading.Tasks;
using Features.Common.StateMachines.Abstract;

namespace Features.GamePlay
{
    public class PaintAnchoring : IPaintAnchoring, IState
    {
        public PaintAnchoring(
            IPaintMover mover,
            IPaintInterceptor interceptor,
            IStateMachine stateMachine,
            IPaintDrop drop,
            PaintAnchoringDefinition definition)
        {
            Definition = definition;
            _mover = mover;
            _interceptor = interceptor;
            _stateMachine = stateMachine;
            _drop = drop;
        }

        private readonly IPaintMover _mover;
        private readonly IPaintInterceptor _interceptor;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintDrop _drop;

        public IStateDefinition Definition { get; }

        public async UniTask Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            
            await _mover.TransitTo(handle.Lifetime, target.CenterTransform);
            await _drop.Enter(target);
            
            _interceptor.Detach();
            _interceptor.Attach(target);
        }
    }
}