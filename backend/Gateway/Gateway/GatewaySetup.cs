namespace Gateway;

public class GatewaySetup : BackgroundService
{
    public GatewaySetup(ILogger<GatewaySetup> logger)
    {
        _logger = logger;
    }

    private readonly ILogger<GatewaySetup> _logger;
    
    protected override Task ExecuteAsync(CancellationToken cancellation)
    {
        _logger.LogInformation("[Gateway] Setup completed");
        return Task.CompletedTask;
    }
}