using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddServiceDefaults()
    .AddOrleans();

//builder.Logging.AddFilter("Orleans.Storage.AdoNetGrainStorage", LogLevel.None);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.Run();