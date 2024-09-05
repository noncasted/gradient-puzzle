using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintMover
    {
        UniTask TransitTo(IReadOnlyLifetime lifetime, Transform target);
        UniTask FollowCursor(IReadOnlyLifetime lifetime, IPaintTarget from);
    }
}