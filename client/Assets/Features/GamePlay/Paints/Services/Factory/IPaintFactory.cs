using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    public interface IPaintFactory
    {
        UniTask<IPaint> Create(IReadOnlyLifetime lifetime, Color color);
    }
}