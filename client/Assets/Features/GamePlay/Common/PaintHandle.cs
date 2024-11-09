using System;
using GamePlay.Paints;
using Internal;

namespace GamePlay.Common
{
    public class PaintHandle : IPaintHandle
    {
        private readonly ViewableProperty<IPaint> _paint = new(null);
        private bool _isLocked;
        
        public IViewableProperty<IPaint> Paint =>_paint;
        public bool IsLocked =>_isLocked;
        
        public void Set(IPaint paint)
        {
            _paint.Set(paint);
        }

        public void Clear(IPaint paint)
        {
            if (_paint.Value != paint)
                throw new Exception();
            
            _paint.Set(null);
        }

        public void Lock()
        {
            _isLocked = true;
        }

        public void Unlock()
        {
            _isLocked = false;
        }
    }
}