using Cysharp.Threading.Tasks;

namespace GamePlay.Paints
{
    public interface IPaintComplete
    {
        UniTask Process();
    }
}