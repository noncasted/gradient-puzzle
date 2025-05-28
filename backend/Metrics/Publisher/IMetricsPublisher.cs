namespace Metrics;

public interface IMetricsPublisher
{
    Task Publish(IMetricData dataToInsert);
}