namespace Metrics;

public interface IMetricData
{
    string TableName { get; }
     
    object[] ToArrayOfObjects();
}