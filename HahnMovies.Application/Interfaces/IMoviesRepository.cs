using HahnMovies.Application.Common.Interfaces;
using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Interfaces;

public interface IMoviesRepository : IRepositoryBase<Movies>
{
    
    Task<Movies?> GetMovieByTitleAsync(string title, CancellationToken cancellationToken = default);
    
    Task<bool> MovieExistsAsync(int movieId, CancellationToken cancellationToken = default);
}