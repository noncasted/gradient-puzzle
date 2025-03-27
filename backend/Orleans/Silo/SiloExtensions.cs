using Microsoft.AspNetCore.Mvc;
using Orleans.Configuration;
using Orleans.Providers;
using ServiceDefaults;

namespace Orleans;

public static class SiloExtensions
{
    public static IHostApplicationBuilder AddOrleans(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        TransactionalStateOptions.DefaultLockTimeout = TimeSpan.FromSeconds(5);

        builder.UseOrleans(siloBuilder =>
        {
            var npgsqlConnectionString = configuration.GetConnectionString(ConnectionNames.Postgres)!;

            siloBuilder.UseTransactions();
            
            siloBuilder.UseAdoNetClustering(options => {
                options.Invariant = "Npgsql";
                options.ConnectionString = npgsqlConnectionString;
            });
            
            siloBuilder.AddAdoNetGrainStorage(ProviderConstants.DEFAULT_PUBSUB_PROVIDER_NAME, options =>
            {
                options.Invariant = "Npgsql";
                options.ConnectionString = npgsqlConnectionString;
            });

            siloBuilder.AddAdoNetGrainStorageAsDefault(options =>
            {
                options.Invariant = "Npgsql";
                options.ConnectionString = npgsqlConnectionString;
            });

            siloBuilder.Configure<GrainCollectionOptions>(options =>
            {
                options.CollectionAge = TimeSpan.FromMinutes(2);
            });

            siloBuilder.AddActivityPropagation();
        });

        return builder;
    }
    
    public static IEndpointRouteBuilder AddSiloHealthcheck(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("silo-health", ([FromServices] ISiloSetup setup) =>
        {
            if (setup.IsStarted == true)
                return Results.Ok();

            return Results.Problem();
        });

        return builder;
    }
}