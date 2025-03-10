using UnityEngine;

namespace Internal
{
    public enum Direction4
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public enum Direction8
    {
        Up,
        Right,
        Down,
        Left
    }

    public static class SideExtensions
    {
        public static Direction4 ToDirection4(this Vector2 vector)
        {
            if (vector.x > 0)
            {
                return Direction4.Right;
            }

            if (vector.x < 0)
            {
                return Direction4.Left;
            }

            if (vector.y > 0)
            {
                return Direction4.Up;
            }

            return Direction4.Down;
        }
    }
}