using System.Collections.Generic;
using GamePlay.Common;
using Internal;
using Services;

namespace GamePlay.Paints
{
    public class DesktopPaintDragStarter : IPaintDragStarter
    {
        public DesktopPaintDragStarter(IGameInput input)
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
                        _selected = target;
                    else
                        TryResetTouch();
                });
            }

            _input.Action.AdviseTrue(lifetime, OnActionPressed);

            void TryResetTouch()
            {
                foreach (var target in targets)
                {
                    if (target.IsTouched.Value == true)
                        return;
                }
                
                _selected = null;
            }
        }

        private void OnActionPressed()
        {
            if (_selected?.GetPaint() == null)
                return;

            _selected.GetPaint().Drag();
        }
    }
}