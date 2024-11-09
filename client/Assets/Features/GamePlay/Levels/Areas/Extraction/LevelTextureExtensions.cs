using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GamePlay.Levels
{
    public static class LevelTextureExtensions
    {
        public static IReadOnlyList<AreaExtractedColor> GetColors(this Texture2D texture, byte epsilon)
        {
            var pixels = texture.GetPixels32();
            var colors = new List<AreaExtractedColor>();

            foreach (var color in pixels)
            {
                if (color.a < 255)
                    continue;
                
                if (IsUnique(color) == false)
                    continue;

                var extractedColor = new AreaExtractedColor(color, epsilon);
                colors.Add(extractedColor);
            }

            return colors.ToList();

            bool IsUnique(Color32 color)
            {
                foreach (var check in colors)
                {
                    if (check.IsEqual(color))
                        return false;
                }

                return true;
            }
        }
    }
}