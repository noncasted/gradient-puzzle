using System.Collections.Generic;
using Features.Services.Inputs;
using Internal;

namespace Features.GamePlay
{
    public class PaintDragStarter : IPaintDragStarter
    {
        public PaintDragStarter(IGameInput input)
        {
            _input = input;
        }

        private readonly IGameInput _input;

        private IPaintTarget _selected;
        private IViewableProperty<bool> _action;

        public IPaintTarget Selected => _selected;

        public void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets)
        {
            foreach (var target in targets)
            {
                target.IsTouched.Advise(lifetime, isTouched =>
                {
                    if (isTouched == true)
                    {
                        _selected = target;
                    }
                    else
                    {
                        if (_selected == target)
                            _selected = null;
                    }
                });
            }

            _input.Action.AdviseTrue(lifetime, OnActionPressed);
        }

        private void OnActionPressed()
        {
            if (_selected?.GetPaint() == null)
                return;

            _selected.GetPaint().Drag();
        }
    }
}