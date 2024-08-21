using Internal;
using UnityEngine;

namespace Features.Services.Inputs
{
    [DisallowMultipleComponent]
    public class GamePlayInputPositionConverter : MonoBehaviour, ISceneService, IGamePlayInputPositionConverter
    {
        [SerializeField] private RectTransform _area;
        [SerializeField] private Camera _camera;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IGamePlayInputPositionConverter>();
        }

        public Vector2 ScreenToLocal(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _area,
                screenPosition,
                _camera,
                out var localPosition);
            
            return localPosition;
        }
    }
}