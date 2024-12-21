using Cysharp.Threading.Tasks;

namespace GamePlay.Paints.Collection
{
    public interface IPaintCollection
    {
        UniTask Initialize();
        void Add(IPaint paint);
        UniTask DestroyAll();
    }
}