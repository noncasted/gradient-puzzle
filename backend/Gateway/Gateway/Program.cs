using Gateway;
using ServiceDefaults;
using Users;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults()
    .AddSecrets()
    .AddOrleans()
    .ConfigureCors();

builder.Services
    .AddOpenApi()
    .AddHostedService<TelegramBot>()
    .AddHostedService<GatewaySetup>();

var app = builder.Build();

app.UseHttpsRedirection();
app.AddUserEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "api"));
}

app.UseCors("cors");
app.UseRouting();

app.Run();