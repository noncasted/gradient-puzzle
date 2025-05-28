using Common;
using Metrics;
using ServiceDefaults;

namespace Gateway;

public static class GatewayExtensions
{
    public static IHostApplicationBuilder AddSecrets(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment() == false)
            return builder;

        builder.Configuration.AddJsonFile("secrets.json");

        return builder;
    }

    public static IHostApplicationBuilder AddTelegram(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment() == true)
            return builder;

        var token = builder.ExtractString("TELEGRAM_TOKEN");

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

        services.AddSingleton<IMigrationMetadata, LevelPassMetrics.Migration>();
        services.AddSingleton<IMigrationMetadata, LevelRateMetrics.Migration>();

        services.AddSingleton(options);
        services.AddHostedSingleton<IMetricsPublisher, MetricsPublisher>();

        return builder;

        string GetConnectionString()
        {
            if (builder.Environment.IsDevelopment() == true)
                return builder.Configuration.GetConnectionString("default")!;

            return builder.Configuration.GetConnectionString(ConnectionNames.ClickHouse)!;
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