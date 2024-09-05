using Internal;
using UnityEngine;

namespace Features.GamePlay
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