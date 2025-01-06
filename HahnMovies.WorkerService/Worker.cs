using HahnMovies.Application.Interfaces;
using Hangfire;

namespace HahnMovies.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _recurringJobManager = recurringJobManager;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

        // Configure and schedule Hangfire recurring jobs
        ScheduleJobs();

        // Hangfire will handle job execution, so no need for a loop here
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private void ScheduleJobs()
    {
        _logger.LogInformation("Scheduling Hangfire jobs...");

        // Schedule the FullSyncFromDailyExport job to run weekly
        _recurringJobManager.AddOrUpdate<IMovieService>(
            "full-sync-tmdb",
            movieService => movieService.FullSyncFromDailyExportAsync(CancellationToken.None),
            Cron.Weekly);

        _logger.LogInformation("Hangfire jobs scheduled successfully.");
        
        // Schedule the PartialSyncFromChanges job to run daily
        _recurringJobManager.AddOrUpdate<IMovieService>(
            "partial-sync-tmdb",
            movieService => movieService.PartialSyncFromChangesAsync(CancellationToken.None),
            Cron.Daily);

        _logger.LogInformation("Hangfire jobs scheduled successfully.");
    }
}