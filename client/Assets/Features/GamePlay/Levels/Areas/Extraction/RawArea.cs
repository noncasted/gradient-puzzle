using UnityEngine;

namespace GamePlay.Levels
{
    public class RawArea
    {
        public RawArea(bool[,] value)
        {
            Value = value;
            Size = new Vector2Int(value.GetLength(1), value.GetLength(0));
        }

        public readonly bool[,] Value;
        public readonly Vector2Int Size;

        public void Set(Vector2Int point)
        {
            Value[point.y, point.x] = true;
        }
        
        public void Clear(Vector2Int point)
        {
            Value[point.y, point.x] = false;
        }
    }
}
