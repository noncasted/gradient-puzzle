namespace Metrics;

public interface IMetricsPublisher
{
    Task Publish(IMigrationMetadata migrationMetadata, IMetricData dataToInsert);
}