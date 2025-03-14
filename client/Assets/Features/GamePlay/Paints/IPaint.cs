using Cysharp.Threading.Tasks;
using GamePlay.Common;
using UnityEngine;

namespace GamePlay.Paints
{
    public interface IPaint
    {
        Color Color { get; }
        GameObject GameObject { get; }
        
        void Construct(Color color);
        
        UniTask Spawn(IPaintTarget target);
        void Drag();
        UniTask Anchor(IPaintTarget target);
        UniTask Complete();
        UniTask Destroy();

    }
}