using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.Services.Inputs;
using Internal;

namespace Features.GamePlay
{
    public class PaintMover : IPaintMover
    {
        public PaintMover(IGameInput input)
        {
            _input = input;
        }

        private readonly IGameInput _input;

        private IPaintTarget _currentTarget;
        private IViewableProperty<bool> _action;
        private IReadOnlyLifetime _parentLifetime;

        public void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets)
        {
            _parentLifetime = lifetime;

            foreach (var target in targets)
            {
                target.IsTouched.Advise(lifetime, isTouched =>
                {
                    if (isTouched == true)
                    {
                        _currentTarget = target;
                    }
                    else
                    {
                        if (_currentTarget == target)
                            _currentTarget = null;
                    }
                });
            }

            _input.Action.AdviseTrue(lifetime, OnActionChanged);
        }

        private void OnActionChanged()
        {
            if (_currentTarget == null || _currentTarget.Paint == null)
                return;

            HandleDrag(_currentTarget).Forget();
        }

        private async UniTask HandleDrag(IPaintTarget startTarget)
        {
            var paint = startTarget.Paint;
            startTarget.RemovePaint(paint);
            var handle = new PaintMoveHandle(_parentLifetime.Child(), startTarget);
            paint.Drag(handle);

            await _input.Action.WaitFalse(handle.Lifetime);

            handle.Dispose();
            
            if (_currentTarget != null && _currentTarget.Paint == null)
            {
                _currentTarget.SetPaint(paint);
                paint.Drop(_currentTarget);
            }
            else
            {
                startTarget.SetPaint(paint);
                paint.Return(startTarget);
            }
        }
    }
}