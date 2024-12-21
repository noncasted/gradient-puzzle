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
            PaintAnchoringDefinition definition)
        {
            _mover = mover;
            _interceptor = interceptor;
            _stateMachine = stateMachine;
            _drop = drop;
            Definition = definition;
        }

        private readonly IPaintMover _mover;
        private readonly IPaintInterceptor _interceptor;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintDrop _drop;

        public IStateDefinition Definition { get; }

        public async UniTask Enter(IPaintTarget target)
        {
            var handle = _stateMachine.CreateHandle(this);
            var from = _interceptor.Current;

            _interceptor.Detach();
            _interceptor.Attach(target);
            
            await _mover.TransitTo(handle.Lifetime, target.CenterTransform, from);
            await _drop.Enter(target);
        }
    }
}