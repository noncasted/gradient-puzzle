using Cysharp.Threading.Tasks;
using Internal;

namespace Features
{
    public interface IGameLoop
    {
        UniTask Process(IReadOnlyLifetime lifetime);
    }
}