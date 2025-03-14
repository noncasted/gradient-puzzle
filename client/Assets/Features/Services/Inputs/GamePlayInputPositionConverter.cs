﻿using Internal;
using UnityEngine;

namespace Services
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

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _area,
                screenPosition,
                _camera,
                out var worldPosition);
            
            return worldPosition;
        }

        public Vector2 ScreenToLocal(RectTransform rect, Vector2 screenPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                screenPosition,
                _camera,
                out var localPosition);

            return localPosition;
        }
    }
}