using System;
using Cysharp.Threading.Tasks;
using Internal;

namespace Global.Backend
{
    public interface IAuthBackend
    {
        UniTask<Guid> Auth(IReadOnlyLifetime lifetime);
    }
}