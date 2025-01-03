using HahnMovies.Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace HahnMovies.Infrastructure.Services
{
    public class HangfireJobScheduler : IJobScheduler
    {
        private readonly IConfiguration _configuration;
        private readonly IMovieService _movieService;

        public HangfireJobScheduler(IConfiguration configuration, IMovieService movieService)
        {
            _configuration = configuration;
            _movieService = movieService;
        }

        public void ScheduleFullSyncJob()
        {
            var cronExpression = _configuration["Hangfire:FullSyncCron"];
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                RecurringJob.AddOrUpdate(
                    "FullSyncFromTMDB",
                    () => _movieService.FullSyncFromDailyExportAsync(CancellationToken.None),
                    cronExpression);
            }
        }
    }
}