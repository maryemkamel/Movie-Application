using Microsoft.EntityFrameworkCore;
using HahnMovies.Application.Interfaces;
using HahnMovies.Domain.Models;
using HahnMovies.Infrastructure.Data;

namespace HahnMovies.Infrastructure.Repositories;


public class MoviesRepository(AppDbContext dbContext) : RepositoryBase<Movies>(dbContext), IMoviesRepository
{
    public Task<Movies?> GetMovieByTitleAsync(string title, CancellationToken cancellationToken = default)
        => _dbSet.FirstOrDefaultAsync(m => m.Title == title, cancellationToken);

    public Task<bool> MovieExistsAsync(int movieId, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(m => m.Id == movieId, cancellationToken);
}