using Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BuildApi>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<BuildApi>());

builder.Configuration.AddJsonFile("secrets.json");
builder.Services.Configure<SecretsOptions>(builder.Configuration.GetSection("Secrets"));

var app = builder.Build();

if (app.Environment.IsDevelopment() == true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();