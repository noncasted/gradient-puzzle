using System;
using Cysharp.Threading.Tasks;
using Internal;

namespace Global.GameServices
{
    public interface IGlobalContext
    {
        Guid UserId { get; }

        UniTask Init(IReadOnlyLifetime lifetime);
    }
}