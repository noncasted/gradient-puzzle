using Cysharp.Threading.Tasks;
using GamePlay.Common;

namespace GamePlay.Paints
{
    public interface IPaintAnchoring
    {
        UniTask Enter(IPaintTarget target);
    }
}