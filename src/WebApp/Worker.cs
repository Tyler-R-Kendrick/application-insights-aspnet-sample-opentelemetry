namespace WebApp;

public class BaseWorker : BackgroundService
{
    readonly Guid _traceID = Guid.NewGuid();
    private readonly ILogger<BackgroundService> _logger;
    private readonly Func<TimeSpan> _delay;

    public BaseWorker(ILogger<BackgroundService> logger, Func<TimeSpan> delay)
    {
        _logger = logger;
        _delay = delay;
    }

    private string LogMessage => $"Worker {_traceID} running at: {{time}}";
    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(LogMessage, DateTimeOffset.Now);
            }
            try
            {
                await OnExecute(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);
            } 
            await Task.Delay(_delay(), stoppingToken);
        }
    }

    protected virtual Task OnExecute(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}

public class Worker(ILogger<Worker> logger)
    : BaseWorker(logger, () => TimeSpan.FromSeconds(1))
    {
    }
public class ExceptionWorker(ILogger<ExceptionWorker> logger)
    : BaseWorker(logger, () => TimeSpan.FromSeconds(Random.Shared.Next(1, 10)))
{
    private readonly Random random = new();
    protected override Task OnExecute(CancellationToken stoppingToken)
    {
        throw new Exception("Worker Exception!");
    }
}