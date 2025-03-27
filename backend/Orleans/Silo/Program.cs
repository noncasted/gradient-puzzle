using Common;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults()
    .AddOrleans();

builder.Services
    .AddHostedSingleton<ISiloSetup, SiloSetup>();
//builder.Logging.AddFilter("Orleans.Storage.AdoNetGrainStorage", LogLevel.None);

var app = builder.Build();

app.UseHttpsRedirection();
app.AddSiloHealthcheck();

app.UseRouting();

app.Run();