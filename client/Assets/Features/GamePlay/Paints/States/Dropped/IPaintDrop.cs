using Cysharp.Threading.Tasks;
using GamePlay.Common;

namespace GamePlay.Paints
{
    public interface IPaintDrop
    {
        UniTask Enter(IPaintTarget target);
    }
}