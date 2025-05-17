namespace Services
{
    public enum LevelSectionType
    {
        Beginner = 10,
        Medium = 20,
        Advanced = 30,
    }

    public static class LeveSectionExtensions
    {
        public static string ToName(this LevelSectionType type)
        {
            return type switch
            {
                LevelSectionType.Beginner => "Beginner",
                LevelSectionType.Medium => "Medium",
                LevelSectionType.Advanced => "Advanced",
                _ => string.Empty
            };
        }
    }
}