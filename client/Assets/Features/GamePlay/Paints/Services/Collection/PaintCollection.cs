using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace GamePlay.Paints.Collection
{
    public class PaintCollection : IPaintCollection
    {
        private readonly List<IPaint> _paints = new();
        
        public async UniTask Initialize()
        {
            await DestroyAll();
        }

        public void Add(IPaint paint)
        {
            _paints.Add(paint);
        }

        public async UniTask DestroyAll()
        {
            foreach (var paint in _paints)
                await paint.Destroy();
            
            _paints.Clear();
        }
    }
}