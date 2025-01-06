using HahnMovies.Application.Interfaces;
using HahnMovies.WorkerService;
using HahnMovies.Infrastructure;
using HahnMovies.Infrastructure.Data;
using HahnMovies.Infrastructure.Services;
using MediatR;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

// Register database and data services
builder.Services.RegisterDataServices(builder.Configuration);

// Add MediatR (if needed)
builder.Services.AddMediatR(typeof(AppDbContext).Assembly);

// Add Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Application")));
builder.Services.AddHangfireServer();

// Register services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITmdbService, TmdbService>();
builder.Services.AddScoped<IJobScheduler, HangfireJobScheduler>();

// Add Hosted Service (Worker)
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Schedule jobs
var serviceProvider = host.Services.CreateScope().ServiceProvider;
var jobScheduler = serviceProvider.GetRequiredService<IJobScheduler>();
jobScheduler.ScheduleFullSyncJob();



host.Run();