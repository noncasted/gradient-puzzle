using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Internal;

namespace Global.Backend
{
    public interface IUserBackend
    {
        UniTask<IReadOnlyDictionary<int, int>> GetProgress(IReadOnlyLifetime lifetime);
        UniTask SaveProgress(IReadOnlyLifetime lifetime, int section, int level);
    }   
}