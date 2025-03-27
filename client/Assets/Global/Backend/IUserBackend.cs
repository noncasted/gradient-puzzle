using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Internal;

namespace Global.Backend
{
    public interface IUserBackend
    {
        UniTask<IReadOnlyList<string>> GetProgress(IReadOnlyLifetime lifetime);
        UniTask SaveProgress(IReadOnlyLifetime lifetime, string passedLevelId);
    }   
}