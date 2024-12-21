using System.Collections.Generic;
using GamePlay.Common;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Paints
{
    public class MobilePaintDragStarter : IPaintDragStarter
    {
        public MobilePaintDragStarter(IGameInput input)
        {
            _input = input;
        }

        private readonly IGameInput _input;

        private IPaintTarget _selected;
        private IViewableProperty<bool> _action;
        private IPaint _currentPaint;

        public IPaintTarget Selected => _selected;

        public void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets)
        {
            _selected = null;
            
            foreach (var target in targets)
            {
                target.IsTouched.Advise(lifetime, isTouched =>
                {
                    if (isTouched == true)
                    {
                        _selected = target;
                        OnActionPressed(true);
                    }
                    else
                    {
                        if (_selected == target)
                            _selected = null;
                    }
                });
            }

            _input.Action.Advise(lifetime, OnActionPressed);
        }

        private void OnActionPressed(bool isPressed)
        {
            if (isPressed == false)
            {
                _currentPaint = null;
                return;
            }
            
            if (_selected == null)
                return;
            
            if (_input.Action.Value == false)
                return;
            
            if (_currentPaint != null)
                return;
            
            if (_selected.GetPaint() == null)
                return;

            _currentPaint = _selected.GetPaint();
            _currentPaint.Drag();
        }
    }
}