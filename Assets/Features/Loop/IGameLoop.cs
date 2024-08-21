using Cysharp.Threading.Tasks;
using Internal;

namespace Features.Loop
{
    public interface IGameLoop
    {
        UniTask Process(IReadOnlyLifetime lifetime);
    }
}