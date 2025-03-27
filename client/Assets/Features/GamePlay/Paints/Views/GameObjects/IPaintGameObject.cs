using UnityEngine;

namespace GamePlay.Paints.GameObjects
{
    public interface IPaintGameObject
    {
        GameObject GameObject { get; }
        
        void DestroySelf();
    }
}