using System;
using Cysharp.Threading.Tasks;
using Global.Backend;
using Global.Publisher;
using Global.Saves;
using Internal;

namespace Global.GameServices
{
    public class GlobalContext : IGlobalContext
    {
        public GlobalContext(IAuthBackend authBackend)
        {
            _authBackend = authBackend;
        }

        private readonly IAuthBackend _authBackend;

        public Guid UserId { get; private set; }

        public async UniTask Init(IReadOnlyLifetime lifetime)
        {
            var userId = await _authBackend.Auth(lifetime);
            UserId = userId;
        }
    }
}