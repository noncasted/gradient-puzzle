using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaint
    {
        Color Color { get; }
        
        void Construct(Color color);
        
        UniTask Spawn(IPaintTarget target);
        void Drag();
        UniTask Anchor(IPaintTarget target);
        UniTask Complete();
        UniTask Destroy();

    }
}