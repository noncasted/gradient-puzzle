using Common;
using Metrics;
using ServiceDefaults;

namespace Gateway;

public static class GatewayExtensions
{
    public static IHostApplicationBuilder AddTelegram(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment() == true)
            return builder;
        
        var token = builder.ExtractString("TelegramToken");

        builder.Services.AddSingleton(new TelegramOptions
        {
            Token = token
        });

        builder.Services.AddHostedService<TelegramBot>();

        return builder;
    }

    public static IHostApplicationBuilder AddOrleans(this IHostApplicationBuilder builder)
    {
        builder.UseOrleansClient(clientBuilder =>
        {
            var npgsqlConnectionString = clientBuilder.Configuration.GetConnectionString(ConnectionNames.Postgres)!;

            clientBuilder.UseTransactions();

            clientBuilder.UseAdoNetClustering(options =>
            {
                options.Invariant = "Npgsql";
                options.ConnectionString = npgsqlConnectionString;
            });
        });

        return builder;
    }

    public static IHostApplicationBuilder AddMetrics(this IHostApplicationBuilder builder)
    {
        var options = new ClickHouseOptions
        {
            ConnectionString = GetConnectionString()
        };

        var services = builder.Services;

        services.AddSingleton<IMigrationMetadata, LevelMetrics.Migration>();

        services.AddSingleton(options);
        services.AddHostedSingleton<IMetricsPublisher, MetricsPublisher>();

        return builder;

        string GetConnectionString()
        {
            if (builder.Environment.IsDevelopment() == true)
                return builder.Configuration.GetConnectionString("default")!;

            var host = builder.ExtractString("ClickHouse_Host");
            var username = builder.ExtractString("ClickHouse_Login");
            var password = builder.ExtractString("ClickHouse_Password");

            return $"Host={host};Protocol=http;Port=8123;Username={username};Password={password};";
        }
    }

    public static void ConfigureCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("cors", policy =>
            {
                if (builder.Environment.IsDevelopment() == true)
                {
                    policy.SetIsOriginAllowed(origin =>
                            Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Host == "localhost")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
                else
                {
                    var url = Environment.GetEnvironmentVariable("BUILD_URL")!;

                    policy.WithOrigins(url)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            });
        });
    }

    private static string ExtractString(this IHostApplicationBuilder builder, string key)
    {
        if (builder.Environment.IsDevelopment() == true)
            return builder.Configuration[key]!;

        return Environment.GetEnvironmentVariable(key)!;
    }
}