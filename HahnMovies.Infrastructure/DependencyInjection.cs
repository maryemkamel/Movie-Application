using HahnMovies.Application.Common.Interfaces;
using HahnMovies.Application.Interfaces;
using HahnMovies.Infrastructure.Data;
using HahnMovies.Infrastructure.Repositories;
using HahnMovies.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HahnMovies.Infrastructure;

public static class  DependencyInjection
{
    public static void RegisterDataServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Application");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        services.AddScoped<IUnitOfWork>(c => c.GetRequiredService<AppDbContext>());
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        // Register Repositories
        services.AddScoped<IMoviesRepository, MoviesRepository>();
        services.AddScoped<IMovieService, MovieService>();
        // Register Services
        services.AddHttpClient<ITmdbService, TmdbService>(client =>
        {
            client.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        });
    }
}