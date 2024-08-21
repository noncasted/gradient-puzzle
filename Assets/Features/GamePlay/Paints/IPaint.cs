using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaint
    {
        void Construct(Color color);
        
        UniTask Spawn(IPaintTarget target);
        UniTask Destroy();
    }
}