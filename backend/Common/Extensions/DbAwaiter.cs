using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using ServiceDefaults;

namespace Common;

public class DbAwaiter : BackgroundService
{
    public DbAwaiter(IConfiguration configuration, ILogger<DbAwaiter> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private readonly IConfiguration _configuration;
    private readonly ILogger<DbAwaiter> _logger;

    protected override async Task ExecuteAsync(CancellationToken cancellation)
    {
        _logger.LogInformation("Wait for db");

        var connectionString = _configuration.GetConnectionString(ConnectionNames.Postgres);
        var safeGuard = 0;

        while (safeGuard < 10)
        {
            safeGuard++;

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cancellation);
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to connect to database: {Message}", e.Message);
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellation);
        }

        throw new Exception();
    }
}

public static class DbAwaiterExtensions
{
    public static IHostApplicationBuilder AddDbAwaiter(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHostedService<DbAwaiter>();
        return builder;
    }
}