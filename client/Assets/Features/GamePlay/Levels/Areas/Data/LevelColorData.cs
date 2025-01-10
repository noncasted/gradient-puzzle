using UnityEngine;

namespace GamePlay.Levels
{
    public class LevelColorData
    {
        public LevelColorData(Color color, Vector2 position)
        {
            Color = color;
            Position = position;
        }

        public Color Color { get; }
        public Vector2 Position { get; }
    }
}