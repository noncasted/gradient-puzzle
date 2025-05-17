using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Internal;

namespace Global.Backend
{
    public interface IUserBackend
    {
        UniTask<IReadOnlyDictionary<int, int>> GetProgress(IReadOnlyLifetime lifetime);
        UniTask SaveProgress(IReadOnlyLifetime lifetime, string passedLevelId);
    }   
}