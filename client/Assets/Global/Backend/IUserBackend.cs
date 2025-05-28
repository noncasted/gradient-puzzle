using System.Collections.Generic;
using System.Net;
using Cysharp.Threading.Tasks;
using Internal;
using Shared;

namespace Global.Backend
{
    public interface IUserBackend
    {
        UniTask<IReadOnlyDictionary<LevelSectionType, int>> GetProgress(IReadOnlyLifetime lifetime);
        UniTask SaveProgress(IReadOnlyLifetime lifetime, LevelSectionType section, int level);
    }   
}