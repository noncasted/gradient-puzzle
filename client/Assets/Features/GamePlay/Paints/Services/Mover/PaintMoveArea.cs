using Internal;
using UnityEngine;

namespace GamePlay.Paints
{
    [DisallowMultipleComponent]
    public class PaintMoveArea : MonoBehaviour, ISceneService, IPaintMoveArea
    {
        [SerializeField] private RectTransform _transform;
        
        public RectTransform Transform => _transform;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintMoveArea>();
        }
    }
}