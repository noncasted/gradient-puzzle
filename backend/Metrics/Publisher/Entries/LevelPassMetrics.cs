using Shared;

namespace Metrics;

public static class LevelPassMetrics
{
    public const string Table = "level_pass";

    public static Payload ToPayload(this MetricsContexts.LevelPass context)
    {
        return new Payload
        {
            Name = context.Section.CreateLevelName(context.LevelIndex),
            Section = context.Section.ToName(),
            LevelIndex = context.LevelIndex,
            Time = (int)context.Time.TotalSeconds,
        };
    }

    public class Payload : IMetricData
    {
        public required string Name { get; init; }
        public required string Section { get; init; }
        public required int LevelIndex { get; init; }
        public required int Time { get; init; }

        public string TableName => Table;

        public object[] ToArrayOfObjects()
        {
            return
            [
                Name,
                Section,
                LevelIndex,
                Time
            ];
        }
    }

    public class Migration : MigrationBase, IMigrationMetadata
    {
        public string TableName => Table;

        public IEnumerable<string> GetScripts()
        {
            var scripts = new List<string>();

            var script = string.Format(this.ReadMigrationScript("LevelPassMigration.chsql"), DatabaseName, Table);
            scripts.Add(script);

            return scripts;
        }
    }
}