using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var clickhouse = builder.AddClickHouse("clickhouse")
    .AddDatabase("default");

var startup = builder.AddProject<Startup>("startup")
    .WithReference(postgres)
    .WaitFor(postgres);

var silo = builder.AddProject<Silo>("silo")
    .WaitForCompletion(startup)
    .WithReference(postgres);

builder.AddProject<Gateway>("gateway")
    .WaitFor(silo)
    .WithReference(postgres)
    .WithReference(clickhouse)
    .WithExternalHttpEndpoints();

builder.Build().Run();