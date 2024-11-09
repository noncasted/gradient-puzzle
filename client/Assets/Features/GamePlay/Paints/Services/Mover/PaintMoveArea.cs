using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintMoveArea : MonoBehaviour, ISceneService, IPaintMoveArea
    {
        
        public Transform Transform => transform;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintMoveArea>();
        }
    }
}