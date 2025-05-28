using Gateway;
using Metrics;
using ServiceDefaults;
using Users;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults()
    .AddSecrets()
    .AddTelegram()
    .AddOrleans()
    .AddMetrics()
    .ConfigureCors();

builder.Services
    .AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();

app
    .AddUserEndpoints()
    .AddMetricsEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "api"));
}

app.UseCors("cors");
app.UseRouting();

app.Run();