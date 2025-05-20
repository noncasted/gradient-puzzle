using ServiceDefaults;

namespace Gateway;

public static class GatewayExtensions
{
    public static IHostApplicationBuilder AddSecrets(this IHostApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment() == true)
        {
            builder.Configuration.AddJsonFile("secrets.json");
            var options = builder.Configuration.GetSection("TelegramOptions").Get<TelegramOptions>();
            builder.Services.AddSingleton(options!);
        }
        else
        {
            var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN")!;

            var options = new TelegramOptions()
            {
                Token = token
            };

            builder.Services.AddSingleton(options);
        }

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

    public static void ConfigureCors(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("cors", policy =>
            {
                var url = Environment.GetEnvironmentVariable("BUILD_URL")!;

                policy.WithOrigins(url)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                //
                // if (builder.Environment.IsDevelopment() == true)
                // {
                //     policy.SetIsOriginAllowed(origin =>
                //             Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.Host == "localhost")
                //         .AllowAnyMethod()
                //         .AllowAnyHeader()
                //         .AllowCredentials();
                // }
                // else
                // {
                //     var url = Environment.GetEnvironmentVariable("BUILD_URL")!;
                //
                //     policy.WithOrigins(url)
                //         .AllowAnyMethod()
                //         .AllowAnyHeader()
                //         .AllowCredentials();
                // }
            });
        });
    }
}