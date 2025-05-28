using Shared;

namespace Metrics;

public static class LevelMetrics
{
    public const string TableName = "level";

    public static Payload ToPayload(this MetricsContexts.Level context)
    {
        return new Payload
        {
            Name = $"{context.Section.ToName()}_{context.LevelIndex}",
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
        public override string TableName => LevelMetrics.TableName;

        public IEnumerable<string> GetScripts()
        {
            var scripts = new List<string>();

            var script = string.Format(this.ReadMigrationScript("LevelMigration.chsql"), DatabaseName, TableName);
            scripts.Add(script);

            return scripts;
        }
    }
}