using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaint
    {
        Color Color { get; }
        
        void Construct(Color color);
        
        UniTask Spawn(IPaintTarget target);
        UniTask Destroy();
        void Drag(IPaintMoveHandle moveHandle);
        void Drop(IPaintTarget target);
        void Return(IPaintTarget target);
        void Anchor(IPaintTarget target);
        UniTask Complete();
    }
}