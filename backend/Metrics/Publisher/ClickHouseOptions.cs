namespace Metrics;

public class ClickHouseOptions
{
    public required string ConnectionString { get; set; }
    public string DatabaseName { get; set; } = "default";

    public int ConnectionAttempCount { get; set; } = 5;
    public int OnErrorWaitInMilliseconds { get; set; } = 1000;
}
