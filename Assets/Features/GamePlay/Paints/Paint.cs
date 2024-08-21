using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public class Paint : IPaint
    {
        private readonly IPaintImage _image;
        private readonly IPaintSpawn _spawn;

        public Paint(IPaintImage image, IPaintSpawn spawn)
        {
            _image = image;
            _spawn = spawn;
        }

        public void Construct(Color color)
        {
            _image.SetColor(color);
        }

        public UniTask Spawn(IPaintTarget target)
        {
            return _spawn.Process(target);
        }

        public async UniTask Destroy()
        {
        }
    }
}