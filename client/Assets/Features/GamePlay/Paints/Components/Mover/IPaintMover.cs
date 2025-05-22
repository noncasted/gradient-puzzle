using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    public interface IPaintMover
    {
        UniTask TransitTo(IReadOnlyLifetime lifetime, Vector2 target);
        UniTask FollowCursor(IReadOnlyLifetime lifetime);
    }
}