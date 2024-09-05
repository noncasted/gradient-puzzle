using Cysharp.Threading.Tasks;

namespace Features.GamePlay
{
    public interface IPaintAnchoring
    {
        UniTask Enter(IPaintTarget target);
    }
}