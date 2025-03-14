﻿using Common.StateMachines;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using Internal;
using Services;

namespace GamePlay.Paints
{
    public class PaintDrag : IPaintDrag, IState
    {
        public PaintDrag(
            IPaintMover mover,
            IPaintDragStarter dragStarter,
            IGameInput input,
            IStateMachine stateMachine,
            IPaintInterceptor interceptor,
            IPaintDrop drop,
            IPaintReturn @return,
            IGameContext gameContext,
            IPaintMerging merging,
            PaintDragDefinition definition)
        {
            Definition = definition;
            _mover = mover;
            _dragStarter = dragStarter;
            _input = input;
            _drop = drop;
            _return = @return;
            _gameContext = gameContext;
            _merging = merging;
            _stateMachine = stateMachine;
            _interceptor = interceptor;
        }

        private readonly IPaintMover _mover;
        private readonly IPaintDragStarter _dragStarter;
        private readonly IGameInput _input;
        private readonly IPaintDrop _drop;
        private readonly IPaintReturn _return;
        private readonly IGameContext _gameContext;
        private readonly IPaintMerging _merging;
        private readonly IStateMachine _stateMachine;
        private readonly IPaintInterceptor _interceptor;

        public IStateDefinition Definition { get; }

        public void Enter()
        {
            var handle = _stateMachine.CreateHandle(this);
            Process(handle.Lifetime).Forget();
        }

        private async UniTask Process(IReadOnlyLifetime lifetime)
        {
            var start = _interceptor.Current;
            start.PaintHandle.Lock();
            _interceptor.Detach();

            _merging.Show(lifetime, _gameContext.Targets);
            _mover.FollowCursor(lifetime, start).Forget();
            await _input.Action.WaitFalse(lifetime);
            start.PaintHandle.Unlock();
            
            var selected = _dragStarter.Selected;

            if (selected == null)
            {
                _return.Enter(start);
            }
            else if (selected.GetPaint() != null && selected.PaintHandle.IsLocked == false)
            {
                var target = selected.GetPaint();
                target.Anchor(start).Forget();
                _interceptor.Attach(selected);
                _drop.Enter(selected).Forget();
            }
            else
            {
                _interceptor.Attach(selected);
                _drop.Enter(selected);
            }
        }
    }
}