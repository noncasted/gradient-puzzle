using Cysharp.Threading.Tasks;
using GamePlay.Common;
using GamePlay.Paints.GameObjects;
using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    public class Paint : IPaint
    {
        public Paint(
            IPaintImage image,
            PaintInterceptor interceptor,
            IPaintSpawn spawn,
            IPaintDrag drag,
            IPaintDrop drop,
            IPaintReturn @return,
            IPaintAnchoring anchoring,
            IPaintComplete complete,
            ILifetime lifetime,
            IPaintGameObject gameObject)
        {
            _image = image;
            _interceptor = interceptor;
            _spawn = spawn;
            _drag = drag;
            _drop = drop;
            _return = @return;
            _anchoring = anchoring;
            _complete = complete;
            _lifetime = lifetime;
            _gameObject = gameObject;
        }

        private readonly IPaintImage _image;
        private readonly PaintInterceptor _interceptor;
        private readonly IPaintSpawn _spawn;
        private readonly IPaintDrag _drag;
        private readonly IPaintDrop _drop;
        private readonly IPaintReturn _return;
        private readonly IPaintAnchoring _anchoring;
        private readonly IPaintComplete _complete;
        private readonly ILifetime _lifetime;
        private readonly IPaintGameObject _gameObject;

        private Color _color;

        public Color Color => _color;
        public GameObject GameObject => _gameObject.GameObject;

        public void Construct(Color color)
        {
            _color = color;
            _image.SetColor(color);
            _interceptor.Construct(this);
        }

        public UniTask Spawn(IPaintTarget target)
        {
            return _spawn.Process(target);
        }

        public void Drag()
        {
            _drag.Enter();
        }

        public void Drop(IPaintTarget target)
        {
            _drop.Enter(target);
        }

        public void Return(IPaintTarget target)
        {
            _return.Enter(target);
        }

        public UniTask Anchor(IPaintTarget target)
        {
            return _anchoring.Enter(target);
        }

        public UniTask Complete()
        {
            return _complete.Process();
        }

        public async UniTask Destroy()
        {
            _lifetime.Terminate();
            _gameObject.DestroySelf();
        }
    }
}