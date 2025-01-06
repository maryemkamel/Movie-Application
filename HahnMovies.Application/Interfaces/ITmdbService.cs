using HahnMovies.Application.Dtos;
using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Interfaces
{
    public interface ITmdbService
    {
        Task<TmdbPopularMoviesResponse?> GetPopularMoviesAsync(int page = 1, CancellationToken cancellationToken = default);
        Task<List<int>> GetMovieIdsFromDailyExportAsync(CancellationToken cancellationToken = default);
        Task<TmdbMovieDto?> GetMovieDetailsAsync(int movieId, CancellationToken cancellationToken = default);
        Task<TmdbMovieChangesResponse?> GetMovieChangesAsync(int page = 1, CancellationToken cancellationToken = default);

    }
}