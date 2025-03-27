using Cysharp.Threading.Tasks;
using GamePlay.Common;

namespace GamePlay.Paints
{
    public interface IPaintSpawn
    {
        UniTask Process(IPaintTarget target);
    }
}