using UnityEngine;

namespace Services
{
    public interface IGamePlayInputPositionConverter
    {
        Vector2 ScreenToLocal(Vector2 screenPosition);
        Vector2 ScreenToWorld(Vector2 screenPosition);
    }
}