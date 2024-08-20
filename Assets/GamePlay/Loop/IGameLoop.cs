using Cysharp.Threading.Tasks;
using Internal;

namespace GamePlay.Loop
{
    public interface IGameLoop
    {
        UniTask Process(IReadOnlyLifetime lifetime);
    }
}