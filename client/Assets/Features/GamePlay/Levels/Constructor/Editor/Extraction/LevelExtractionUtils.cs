namespace GamePlay.Levels
{
    public class LevelExtractionUtils
    {
        public static LevelExtractionType Type => LevelExtractionType.SVG;
    }

    public enum LevelExtractionType
    {
        Texture,
        SVG
    }
}