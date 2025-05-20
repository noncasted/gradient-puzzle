using Cysharp.Threading.Tasks;

namespace Global.Publisher
{
    public interface IDataStorage
    {
        T Get<T>() where T : class, new();
        UniTask Save<T>(T data);
    }
}