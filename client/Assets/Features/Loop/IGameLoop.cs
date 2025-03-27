using Cysharp.Threading.Tasks;
using Internal;

namespace Loop
{
    public interface IGameLoop
    {
        UniTask Process(IReadOnlyLifetime lifetime);
    }
}