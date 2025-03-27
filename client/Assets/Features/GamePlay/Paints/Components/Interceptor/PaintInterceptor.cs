using GamePlay.Common;

namespace GamePlay.Paints
{
    public class PaintInterceptor : IPaintInterceptor
    {
        private IPaint _paint;
        private IPaintTarget _current;

        public IPaintTarget Current => _current;
        
        public void Construct(IPaint paint)
        {
            _paint = paint;
        }

        public void LockCurrent()
        {
            _current.PaintHandle.Lock();
        }

        public void UnlockCurrent()
        {
            _current.PaintHandle.Unlock();
        }

        public void Attach(IPaintTarget target)
        {
            _current = target;
            _current.PaintHandle.Set(_paint);
        }

        public void Detach()
        {
            _current.PaintHandle.Clear(_paint);
            _current = null;
        }
    }
}