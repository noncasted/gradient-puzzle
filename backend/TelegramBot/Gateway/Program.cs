using Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<BuildApi>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<BuildApi>());

if (builder.Environment.IsDevelopment() == true)
{
    builder.Configuration.AddJsonFile("secrets.json");
    var options = builder.Configuration.GetSection("Secrets").Get<SecretsOptions>();
    builder.Services.AddSingleton(options!);
}
else
{
    var token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN")!;
    var options = new SecretsOptions()
    {
        Token = token
    };
    builder.Services.AddSingleton(options);
}

var app = builder.Build();

if (app.Environment.IsDevelopment() == true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();