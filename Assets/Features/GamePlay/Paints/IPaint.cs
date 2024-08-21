using Cysharp.Threading.Tasks;

namespace Features.GamePlay.Paints
{
    public interface IPaint
    {
        UniTask Spawn();
        UniTask Destroy();
    }
}