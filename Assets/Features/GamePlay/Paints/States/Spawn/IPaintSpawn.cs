using Cysharp.Threading.Tasks;

namespace Features.GamePlay
{
    public interface IPaintSpawn
    {
        UniTask Process(IPaintTarget target);
    }
}