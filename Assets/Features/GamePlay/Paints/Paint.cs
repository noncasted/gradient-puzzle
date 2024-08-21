using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public class Paint : IPaint
    {
        private readonly IPaintImage _image;
        private readonly IPaintSpawn _spawn;
        private readonly IPaintDrag _drag;
        private readonly IPaintDrop _drop;
        private readonly IPaintReturn _return;

        public Paint(
            IPaintImage image,
            IPaintSpawn spawn,
            IPaintDrag drag,
            IPaintDrop drop,
            IPaintReturn @return)
        {
            _image = image;
            _spawn = spawn;
            _drag = drag;
            _drop = drop;
            _return = @return;
        }

        public void Construct(Color color)
        {
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
        
        public async UniTask Destroy()
        {
        }
    }
}