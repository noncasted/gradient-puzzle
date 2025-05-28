using Shared;

namespace Metrics;

public static class LevelRateMetrics
{
    public const string Table = "level_rate";

    public static Payload ToPayload(this MetricsContexts.LevelRate context)
    {
        return new Payload
        {
            Name = context.Section.CreateLevelName(context.LevelIndex),
            Section = context.Section.ToName(),
            LevelIndex = context.LevelIndex,
            Rate = (int)context.Rate
        };
    }

    public class Payload : IMetricData
    {
        public required string Name { get; init; }
        public required string Section { get; init; }
        public required int LevelIndex { get; init; }
        public required int Rate { get; init; }

        public string TableName => Table;

        public object[] ToArrayOfObjects()
        {
            return
            [
                Name,
                Section,
                LevelIndex,
                Rate
            ];
        }
    }

    public class Migration : MigrationBase, IMigrationMetadata
    {
        public string TableName => Table;

        public IEnumerable<string> GetScripts()
        {
            var scripts = new List<string>();

            var script = string.Format(this.ReadMigrationScript("LevelRateMigration.chsql"), DatabaseName, Table);
            scripts.Add(script);

            return scripts;
        }
    }
}