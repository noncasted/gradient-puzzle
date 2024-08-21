using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintFactory
    {
        UniTask<IPaint> Create(IReadOnlyLifetime lifetime, Color color);
    }
}