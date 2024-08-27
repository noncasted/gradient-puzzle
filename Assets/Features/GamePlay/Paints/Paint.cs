using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public class Paint : IPaint
    {
        public Paint(
            IPaintImage image,
            IPaintSpawn spawn,
            IPaintDrag drag,
            IPaintDrop drop,
            IPaintReturn @return,
            IPaintAnchoring anchoring,
            IPaintComplete complete)
        {
            _image = image;
            _spawn = spawn;
            _drag = drag;
            _drop = drop;
            _return = @return;
            _anchoring = anchoring;
            _complete = complete;
        }

        private readonly IPaintImage _image;
        private readonly IPaintSpawn _spawn;
        private readonly IPaintDrag _drag;
        private readonly IPaintDrop _drop;
        private readonly IPaintReturn _return;
        private readonly IPaintAnchoring _anchoring;
        private readonly IPaintComplete _complete;

        private Color _color;

        public Color Color => _color;

        public void Construct(Color color)
        {
            _color = color;
            _image.SetColor(color);
        }

        public UniTask Spawn(IPaintTarget target)
        {
            return _spawn.Process(target);
        }

        public void Drag(IPaintMoveHandle moveHandle)
        {
            _drag.Enter(moveHandle);
        }

        public void Drop(IPaintTarget target)
        {
            _drop.Enter(target);
        }

        public void Return(IPaintTarget target)
        {
            _return.Enter(target);
        }

        public void Anchor(IPaintTarget target)
        {
            _anchoring.Enter(target);
        }

        public UniTask Complete()
        {
            return _complete.Process();
        }

        public async UniTask Destroy()
        {
        }
    }
}