using Quartz;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class LogMessageJob : IJob
{
    private readonly ILogger<LogMessageJob> _logger;
    public LogMessageJob(ILogger<LogMessageJob> logger)
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Current Time: " + DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
