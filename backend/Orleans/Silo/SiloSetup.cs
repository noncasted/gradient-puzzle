namespace Orleans;

public interface ISiloSetup
{
    bool IsStarted { get; }
}

public class SiloSetup : BackgroundService, ISiloSetup
{
    public SiloSetup(ILogger<SiloSetup> logger)
    {
        _logger = logger;
    }

    private readonly ILogger<SiloSetup> _logger;

    public bool IsStarted { get; private set; }

    protected override Task ExecuteAsync(CancellationToken cancellation)
    {
        _logger.LogInformation("[Silo] Setup completed");
        IsStarted = true;
        return Task.CompletedTask;
    }
}