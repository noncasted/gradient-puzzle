using Cysharp.Threading.Tasks;

namespace Features.GamePlay
{
    public interface IPaintDrop
    {
        UniTask Enter(IPaintTarget target);
    }
}