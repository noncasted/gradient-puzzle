using ClickHouse.Client;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Copy;
using ClickHouse.Client.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Metrics;

public class MetricsPublisher : BackgroundService, IMetricsPublisher
{
    private readonly ClickHouseConnection? _connection;
    private readonly ILogger<MetricsPublisher> _logger;
    private readonly IEnumerable<IMigrationMetadata> _migrations;
    private readonly ClickHouseOptions _options;

    public MetricsPublisher(
        IEnumerable<IMigrationMetadata> migrations,
        ClickHouseOptions databaseOptions,
        ILogger<MetricsPublisher> logger)
    {
        _migrations = migrations;
        _options = databaseOptions;
        _logger = logger;

        _connection = new ClickHouseConnection(_options.ConnectionString);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[Metrics] Starting ClickHouse publisher... Connection string: {ConnectionString}",
            _options.ConnectionString);
        
        while (await IsConnected() == false)
            await Task.Delay(100, stoppingToken);

        foreach (var migration in _migrations)
            migration.SetDatabaseName(_options.DatabaseName);

        var scripts = new List<string>();

        foreach (var migration in _migrations)
            scripts.AddRange(migration.GetScripts());
        
        _logger.LogInformation("[Metrics] Found {MigrationCount} migrations to execute.", scripts.Count);

        foreach (var script in scripts)
            await TryExecute(() => _connection.ExecuteStatementAsync(script));
    }

    public async Task Publish(
        IMigrationMetadata migrationMetadata,
        IMetricData dataToInsert)
    {
        using var bulkCopyInterface = GetBulkCopyInterface(migrationMetadata.TableName, 1);

        await TryExecute(() => bulkCopyInterface.InitAsync());

        try
        {
            await bulkCopyInterface.WriteToServerAsync(new List<object[]>() { dataToInsert.ToArrayOfObjects() });
        }
        catch (ClickHouseServerException ex)
        {
            _logger.LogError(ex.GetBaseException(), "Error while inserting data to ClickHouse: {Message}", ex.Message);
            throw;
        }
    }

    private async Task<bool> IsConnected()
    {
        try
        {
            await TryExecute(Ping);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task Ping()
    {
        await _connection.ExecuteScalarAsync("select 1");
    }

    private async Task TryExecute(Func<Task> execution)
    {
        var attempt = 0;

        while (attempt < _options.ConnectionAttempCount)
        {
            try
            {
                attempt++;
                await execution();
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[Metrics] Error while executing command: {Message}", e.Message);
            }
        }

        if (attempt >= _options.ConnectionAttempCount)
        {
            _logger.LogError("[Metrics] Failed to execute command after {AttemptCount} attempts.", attempt);
            throw new Exception("Failed to execute command after maximum attempts.");
        }
    }

    private ClickHouseBulkCopy GetBulkCopyInterface(string tableName, int recordCount)
    {
        return new ClickHouseBulkCopy(_connection)
        {
            DestinationTableName = string.IsNullOrEmpty(_options.DatabaseName)
                ? $"{tableName}"
                : $"{_options.DatabaseName}.{tableName}",
            BatchSize = recordCount
        };
    }
}